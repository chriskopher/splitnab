using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SplitwiseClient;
using SplitwiseClient.Model.Expenses;
using YnabClient;
using YnabClient.Model.Transactions;

namespace Splitnab
{
    public class Splitnab
    {
        private readonly ILogger<Splitnab> _logger;
        private readonly ISplitwiseClient _splitwiseClient;
        private readonly IYnabClient _ynabClient;

        public Splitnab(ILogger<Splitnab> logger, ISplitwiseClient splitwiseClient, IYnabClient ynabClient)
        {
            _logger = logger;
            _splitwiseClient = splitwiseClient;
            _ynabClient = ynabClient;
        }

        public async Task Run(AppSettings settings, bool dryRun)
        {
            await _splitwiseClient.ConfigureAccessToken(settings.SwConsumerKey, settings.SwConsumerSecret);
            _ynabClient.ConfigureAuthorization(settings.YnabPersonalAccessToken);

            // 1. Get desired Splitwise expenses
            var currentUser = await _splitwiseClient.GetCurrentUser();
            if (currentUser.User == null)
            {
                _logger.LogWarning("Unable to fetch the current Splitwise user. Exiting...");
                return;
            }

            var friends = await _splitwiseClient.GetFriends();
            if (friends.Friends == null)
            {
                _logger.LogWarning("Unable to fetch current user's Splitwise friends list. Exiting...");
                return;
            }

            var friend = friends.Friends.FirstOrDefault(x => x.Email == settings.SwFriendEmail);
            if (friend == null)
            {
                _logger.LogWarning("Unable to find the specified Splitwise friend. Exiting...");
                return;
            }

            var expenses = await _splitwiseClient.GetExpenses(
                friendId: friend.Id,
                datedAfter: settings.SwTransactionsDatedAfter,
                limit: 0);
            if (expenses.Expenses == null)
            {
                _logger.LogWarning("No Splitwise expenses found for the specified user and date range. Exiting...");
                return;
            }

            // 2. Map them as YNAB transactions
            var ynabBudgets = await _ynabClient.GetBudgets(true);
            if (ynabBudgets.Data?.Budgets == null)
            {
                _logger.LogWarning("Unable to fetch YNAB budgets. Exiting...");
                return;
            }

            var ynabBudget = ynabBudgets.Data.Budgets.FirstOrDefault(x => x.Name == settings.YnabBudgetName);
            if (ynabBudget == null)
            {
                _logger.LogWarning("Unable to find YNAB budget. Exiting...");
                return;
            }

            var ynabBudgetAccounts = await _ynabClient.GetBudgetAccounts(ynabBudget.Id);
            var splitwiseAccount = ynabBudgetAccounts.Data?.Accounts?
                .SingleOrDefault(x => x.Name == settings.YnabSplitwiseBudgetName);
            if (splitwiseAccount == null)
            {
                _logger.LogWarning(
                    $"Unable to find YNAB splitwise account {settings.YnabBudgetName} in budget {settings.YnabBudgetName}. Exiting...");
                return;
            }

            var ynabTransactions = expenses.Expenses.Select(expense => new SaveTransaction
            {
                AccountId = splitwiseAccount.Id,
                Date = expense.Date.GetValueOrDefault(),
                Amount = CalculateYnabAmount(expense, currentUser.User.Id),
                PayeeName = string.Join(" ",
                    new[] {friend.FirstName, friend.LastName}.Where(x => !string.IsNullOrWhiteSpace(x))),
                Memo = expense.Description,
                Approved = false
            });

            // 3. Save them as YNAB transactions
            var transactionsToPost = new Transactions {SaveTransactions = ynabTransactions.ToList()};

            if (dryRun)
            {
                foreach (var transaction in transactionsToPost.SaveTransactions)
                {
                    _logger.LogInformation(transaction.ToString());
                }

                _logger.LogInformation(
                    "Dry run completed successfully. No transactions have been imported. To save transactions run without --dry-run flag.");
            }
            else
            {
                await _ynabClient.PostTransactions(ynabBudget.Id, transactionsToPost);
                _logger.LogInformation(
                    $"Successfully saved {transactionsToPost.SaveTransactions.Count} transactions from Splitwise to YNAB using budget {settings.YnabBudgetName} in the {settings.YnabSplitwiseBudgetName} account!");
            }
        }

        private long CalculateYnabAmount(Expense expense, int currentUserId)
        {
            if (expense.Repayments == null)
            {
                _logger.LogWarning(
                    $"Malformed Splitwise expense. Setting cost to 0 for {expense.Description} on {expense.Date}");
                return 0;
            }

            var cost = Convert.ToDouble(expense.Cost, System.Globalization.CultureInfo.InvariantCulture);
            if (expense.CreationMethod != "payment") // Don't split (re)payment transactions
            {
                cost /= 2; // Otherwise, assume equal 2-way split
            }

            if (expense.Repayments[0].From == currentUserId)
            {
                cost = -cost;
            }

            cost *= 1000; // Convert to milli-units

            return Convert.ToInt64(cost);
        }
    }
}

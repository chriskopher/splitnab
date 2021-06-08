using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SplitwiseClient;
using SplitwiseClient.Model.Expenses;
using YnabClient;
using YnabClient.Model.Transactions;

namespace Splitnab
{
    public class SplitnabRunner
    {
        private readonly ILogger<SplitnabRunner> _logger;
        private readonly ISplitwiseClient _splitwiseClient;
        private readonly IYnabClient _ynabClient;

        public SplitnabRunner(ILogger<SplitnabRunner> logger, ISplitwiseClient splitwiseClient, IYnabClient ynabClient)
        {
            _logger = logger;
            _splitwiseClient = splitwiseClient;
            _ynabClient = ynabClient;
        }

        public async Task<Transactions?> Run(AppSettings settings, bool dryRun)
        {
            await _splitwiseClient.ConfigureAccessToken(settings.Splitwise.ConsumerKey,
                settings.Splitwise.ConsumerSecret);
            _ynabClient.ConfigureAuthorization(settings.Ynab.PersonalAccessToken);

            // 1. Get desired Splitwise expenses
            var currentUser = await _splitwiseClient.GetCurrentUser();
            if (currentUser.User == null)
            {
                _logger.LogWarning("Unable to fetch the current Splitwise user. Exiting...");
                return null;
            }

            var friends = await _splitwiseClient.GetFriends();
            if (friends.Friends == null)
            {
                _logger.LogWarning("Unable to fetch current user's Splitwise friends list. Exiting...");
                return null;
            }

            var friend = friends.Friends.FirstOrDefault(x => x.Email == settings.Splitwise.FriendEmail);
            if (friend == null)
            {
                _logger.LogWarning("Unable to find the specified Splitwise friend. Exiting...");
                return null;
            }

            var expenses = await _splitwiseClient.GetExpenses(
                friendId: friend.Id,
                datedAfter: settings.Splitwise.TransactionsDatedAfter,
                limit: 0);
            if (expenses.Expenses == null)
            {
                _logger.LogWarning("No Splitwise expenses found for the specified user and date range. Exiting...");
                return null;
            }

            // 2. Map them as YNAB transactions
            var ynabBudgets = await _ynabClient.GetBudgets(true);
            if (ynabBudgets.Data?.Budgets == null)
            {
                _logger.LogWarning("Unable to fetch YNAB budgets. Exiting...");
                return null;
            }

            var ynabBudget = ynabBudgets.Data.Budgets.FirstOrDefault(x => x.Name == settings.Ynab.BudgetName);
            if (ynabBudget == null)
            {
                _logger.LogWarning("Unable to find YNAB budget. Exiting...");
                return null;
            }

            var ynabBudgetAccounts = await _ynabClient.GetBudgetAccounts(ynabBudget.Id);
            var splitwiseAccount = ynabBudgetAccounts.Data?.Accounts?
                .SingleOrDefault(x => x.Name == settings.Ynab.SplitwiseAccountName);
            if (splitwiseAccount == null)
            {
                _logger.LogWarning(
                    "Unable to find YNAB splitwise account {SplitwiseAccountName} in budget {YnabBudgetName}. Exiting...",
                    settings.Ynab.SplitwiseAccountName, settings.Ynab.BudgetName);
                return null;
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
                    _logger.LogInformation("{Transaction}", transaction.ToString());
                }

                _logger.LogInformation(
                    "Dry run completed successfully. No transactions have been imported. To save transactions run without --dry-run flag");
            }
            else
            {
                await _ynabClient.PostTransactions(ynabBudget.Id, transactionsToPost);
                _logger.LogInformation(
                    "Successfully saved {NumberOfTransactions} transactions from Splitwise to YNAB using budget {YnabBudgetName} in the {YnabSplitwiseAccountName} account!",
                    transactionsToPost.SaveTransactions.Count, settings.Ynab.BudgetName,
                    settings.Ynab.SplitwiseAccountName);
            }

            return transactionsToPost;
        }

        private long CalculateYnabAmount(Expense expense, int currentUserId)
        {
            if (expense.Repayments == null)
            {
                _logger.LogWarning(
                    "Malformed Splitwise expense. Setting cost to 0 for {ExpenseDescription} on {ExpenseDate}",
                    expense.Description, expense.Date);

                return 0;
            }

            var cost = Convert.ToDouble(expense.Cost, CultureInfo.InvariantCulture);
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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SplitwiseClient;
using SplitwiseClient.Model.Expenses;
using SplitwiseClient.Model.Friends;
using SplitwiseClient.Model.Users;
using YnabClient;
using YnabClient.Model.Accounts;
using YnabClient.Model.Budgets;
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
            // Setup authentication for clients
            await _splitwiseClient.ConfigureAccessToken(settings.Splitwise.ConsumerKey,
                settings.Splitwise.ConsumerSecret);
            _ynabClient.ConfigureAuthorization(settings.Ynab.PersonalAccessToken);

            var (swSuccess, (swUser, swFriend, swExpenses)) = await CanGetSplitwiseInfo(settings);
            if (!swSuccess)
            {
                _logger.LogError("Unable to fetch required Splitwise information");
                _logger.LogInformation("Verify that the Splitwise section in appsettings.json is configured correctly");
                return null;
            }

            var (ynabSuccess, (ynabBudget, ynabAccount)) = await CanGetYnabInfo(settings);
            if (!ynabSuccess)
            {
                _logger.LogError("Unable to fetch required YNAB information");
                _logger.LogInformation("Verify that the YNAB section in appsettings.json is configured correctly");
                return null;
            }

            // Map expenses as YNAB transactions
            var ynabTransactions = swExpenses.Select(expense => new SaveTransaction
            {
                AccountId = ynabAccount.Id,
                Date = expense.Date.GetValueOrDefault(),
                Amount = CalculateYnabAmount(expense, swUser.Id),
                PayeeName = string.Join(" ",
                    new[] {swFriend.FirstName, swFriend.LastName}.Where(x => !string.IsNullOrWhiteSpace(x))),
                Memo = expense.Description,
                Approved = false
            });

            // Save YNAB transactions
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

        private async Task<(bool success, (User, FriendModel, List<Expense>) info)> CanGetSplitwiseInfo(
            AppSettings settings)
        {
            // Validate that required Splitwise information is retrievable
            var currentUser = await _splitwiseClient.GetCurrentUser();
            if (currentUser.User == null)
            {
                _logger.LogWarning("Unable to fetch the current Splitwise user");
                return (false, (new User(), new FriendModel(), new List<Expense>()));
            }

            var friends = await _splitwiseClient.GetFriends();
            if (friends.Friends == null)
            {
                _logger.LogWarning("Unable to fetch current user's Splitwise friends list");
                return (false, (currentUser.User, new FriendModel(), new List<Expense>()));
            }

            var friend = friends.Friends?.FirstOrDefault(x => x.Email == settings.Splitwise.FriendEmail);
            if (friend == null)
            {
                _logger.LogWarning("Unable to find the specified Splitwise friend");
                return (false, (currentUser.User, new FriendModel(), new List<Expense>()));
            }

            // Get desired Splitwise expenses
            var expenses = await _splitwiseClient.GetExpenses(
                friendId: friend.Id,
                datedAfter: settings.Splitwise.TransactionsDatedAfter,
                limit: 0);
            if (expenses.Expenses == null)
            {
                _logger.LogWarning("No Splitwise expenses found for the specified user and date range");
                return (false, (currentUser.User, friend, new List<Expense>()));
            }

            return (true, (currentUser.User, friend, expenses.Expenses));
        }

        private async Task<(bool success, (BudgetSummary, Account) info)> CanGetYnabInfo(AppSettings settings)
        {
            // Validate that required YNAB information is retrievable
            var ynabBudgets = await _ynabClient.GetBudgets(true);
            if (ynabBudgets.Data?.Budgets == null)
            {
                _logger.LogWarning("Unable to fetch YNAB budgets");
                return (false, (new BudgetSummary(), new Account()));
            }

            var ynabBudget = ynabBudgets.Data.Budgets.FirstOrDefault(x => x.Name == settings.Ynab.BudgetName);
            if (ynabBudget == null)
            {
                _logger.LogWarning("Unable to find YNAB budget");
                return (false, (new BudgetSummary(), new Account()));
            }

            var ynabBudgetAccounts = await _ynabClient.GetBudgetAccounts(ynabBudget.Id);
            var splitwiseAccount =
                ynabBudgetAccounts.Data?.Accounts?.SingleOrDefault(x => x.Name == settings.Ynab.SplitwiseAccountName);
            if (splitwiseAccount == null)
            {
                _logger.LogWarning(
                    "Unable to find YNAB splitwise account {SplitwiseAccountName} in budget {YnabBudgetName}",
                    settings.Ynab.SplitwiseAccountName, settings.Ynab.BudgetName);
                return (false, (ynabBudget, new Account()));
            }

            return (true, (ynabBudget, splitwiseAccount));
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

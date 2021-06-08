using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Splitnab.Model;
using SplitwiseClient.Model.Expenses;
using YnabClient;
using YnabClient.Model.Accounts;
using YnabClient.Model.Budgets;
using YnabClient.Model.Transactions;

namespace Splitnab
{
    public class SplitnabRunner
    {
        private readonly ILogger<SplitnabRunner> _logger;
        private readonly IGetSplitwiseInfoOperation _getSplitwiseInfoOperation;
        private readonly IYnabClient _ynabClient;

        public SplitnabRunner(ILogger<SplitnabRunner> logger, IGetSplitwiseInfoOperation getSplitwiseInfoOperation,
            IYnabClient ynabClient)
        {
            _logger = logger;
            _getSplitwiseInfoOperation = getSplitwiseInfoOperation;
            _ynabClient = ynabClient;
        }

        public async Task<Transactions?> Run(AppSettings settings, bool dryRun)
        {
            // Setup authentication for clients
            _ynabClient.ConfigureAuthorization(settings.Ynab.PersonalAccessToken);

            var swInfo = await _getSplitwiseInfoOperation.Invoke(settings);
            if (swInfo == null)
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
            var ynabTransactions = swInfo.Expenses.Select(expense => new SaveTransaction
            {
                AccountId = ynabAccount.Id,
                Date = expense.Date.GetValueOrDefault(),
                Amount = CalculateYnabAmount(expense, swInfo.CurrentUser.Id),
                PayeeName = string.Join(" ",
                    new[] {swInfo.Friend.FirstName, swInfo.Friend.LastName}.Where(x => !string.IsNullOrWhiteSpace(x))),
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

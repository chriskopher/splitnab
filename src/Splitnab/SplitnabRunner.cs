using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Splitnab.Model;
using SplitwiseClient.Model.Expenses;
using YnabClient;
using YnabClient.Model.Transactions;

namespace Splitnab
{
    public class SplitnabRunner
    {
        private readonly ILogger<SplitnabRunner> _logger;
        private readonly IGetSplitwiseInfoOperation _getSplitwiseInfoOperation;
        private readonly IGetYnabInfoOperation _getYnabInfoOperation;
        private readonly IYnabClient _ynabClient;

        public SplitnabRunner(ILogger<SplitnabRunner> logger,
            IGetSplitwiseInfoOperation getSplitwiseInfoOperation,
            IGetYnabInfoOperation getYnabInfoOperation,
            IYnabClient ynabClient)
        {
            _logger = logger;
            _getSplitwiseInfoOperation = getSplitwiseInfoOperation;
            _getYnabInfoOperation = getYnabInfoOperation;
            _ynabClient = ynabClient;
        }

        public async Task<Transactions?> Run(AppSettings appSettings, bool dryRun)
        {
            var swInfo = await _getSplitwiseInfoOperation.Invoke(appSettings);
            if (swInfo == null)
            {
                _logger.LogError("Unable to fetch required Splitwise information");
                _logger.LogInformation("Verify that the Splitwise section in appsettings.json is configured correctly");

                return null;
            }

            var ynabInfo = await _getYnabInfoOperation.Invoke(appSettings);
            if (ynabInfo == null)
            {
                _logger.LogError("Unable to fetch required YNAB information");
                _logger.LogInformation("Verify that the YNAB section in appsettings.json is configured correctly");

                return null;
            }

            var transactionsToPost = MapSplitwiseExpensesToYnabTransactions(swInfo, ynabInfo);
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
                _ynabClient.ConfigureAuthorization(appSettings.Ynab.PersonalAccessToken);

                await _ynabClient.PostTransactions(ynabInfo.Budget.Id, transactionsToPost);
                _logger.LogInformation(
                    "Successfully saved {NumberOfTransactions} transactions from Splitwise to YNAB using budget {YnabBudgetName} in the {YnabSplitwiseAccountName} account!",
                    transactionsToPost.SaveTransactions.Count, appSettings.Ynab.BudgetName,
                    appSettings.Ynab.SplitwiseAccountName);
            }

            return transactionsToPost;
        }

        private Transactions MapSplitwiseExpensesToYnabTransactions(SplitwiseInfo swInfo, YnabInfo ynabInfo)
        {
            return new()
            {
                SaveTransactions = swInfo.Expenses.Select(expense => new SaveTransaction
                {
                    AccountId = ynabInfo.SplitwiseAccount.Id,
                    Date = expense.Date.GetValueOrDefault(),
                    Amount = CalculateYnabAmount(expense, swInfo.CurrentUser.Id),
                    PayeeName = string.Join(" ",
                        new[] {swInfo.Friend.FirstName, swInfo.Friend.LastName}.Where(
                            x => !string.IsNullOrWhiteSpace(x))),
                    Memo = expense.Description,
                    Approved = false
                }).ToList()
            };
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

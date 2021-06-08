using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Splitnab;
using Splitnab.Model;
using SplitwiseClient.Model.Expenses;
using SplitwiseClient.Model.Friends;
using SplitwiseClient.Model.Users;
using Xunit;
using YnabClient;
using YnabClient.Model.Accounts;
using YnabClient.Model.Budgets;
using YnabClient.Model.Transactions;

namespace SplitnabTest
{
    public class SplitnabRunnerTest
    {
        private readonly ILogger<SplitnabRunner> _logger;
        private readonly IGetSplitwiseInfoOperation _getSplitwiseInfoOperation;
        private readonly IYnabClient _ynabClient;

        private SplitnabRunner _sut;

        public SplitnabRunnerTest()
        {
            _logger = Substitute.For<ILogger<SplitnabRunner>>();
            _getSplitwiseInfoOperation = Substitute.For<IGetSplitwiseInfoOperation>();
            _ynabClient = Substitute.For<IYnabClient>();
        }

        [Fact]
        public async Task RunWithCorrectValuesForSplitwiseLendingReturnsExpected()
        {
            // Arrange
            var appSettings = new AppSettings
            {
                Splitwise = new Splitwise
                {
                    ConsumerKey = "consumerKey",
                    ConsumerSecret = "consumerSecret",
                    FriendEmail = "friendEmail",
                    TransactionsDatedAfter = new DateTimeOffset()
                },
                Ynab = new Ynab
                {
                    PersonalAccessToken = "personalAccessToken",
                    BudgetName = "budgetName",
                    SplitwiseAccountName = "splitwiseAccountName"
                }
            };

            var expenseDate = DateTime.Now;
            var expectedSplitwiseInfo = new SplitwiseInfo
            {
                CurrentUser = new User {Id = 1, FirstName = "firstName"},
                Friend = new FriendModel {Id = 2, FirstName = "friendName", Email = "friendEmail"},
                Expenses = new List<Expense>
                {
                    new()
                    {
                        Cost = "123450",
                        Date = expenseDate,
                        Description = "expensive",
                        Repayments = new List<Repayment> {new() {From = 2}}
                    }
                }
            };
            _getSplitwiseInfoOperation.Invoke(appSettings).Returns(expectedSplitwiseInfo);

            var expectedYnabBudgetGuid = Guid.NewGuid();
            var expectedBudgets = new BudgetSummaryResponse
            {
                Data = new BudgetModel
                {
                    Budgets = new List<BudgetSummary> {new() {Name = "budgetName", Id = expectedYnabBudgetGuid}}
                }
            };
            _ynabClient.GetBudgets(true).Returns(expectedBudgets);

            var expectedYnabBudgetAccountGuid = Guid.NewGuid();
            var expectedBudgetAccounts = new AccountsResponse
            {
                Data = new AccountsModel
                {
                    Accounts = new List<Account>
                    {
                        new() {Name = "splitwiseAccountName", Id = expectedYnabBudgetAccountGuid}
                    }
                }
            };
            _ynabClient.GetBudgetAccounts(expectedYnabBudgetGuid).Returns(expectedBudgetAccounts);

            var expectedTransactions = new Transactions
            {
                SaveTransactions = new List<SaveTransaction>
                {
                    new()
                    {
                        AccountId = expectedYnabBudgetAccountGuid,
                        Date = expenseDate,
                        Amount = 61725000,
                        PayeeName = "friendName",
                        Memo = "expensive",
                        Approved = false
                    }
                }
            };

            // Act
            _sut = new SplitnabRunner(_logger, _getSplitwiseInfoOperation, _ynabClient);
            var result = await _sut.Run(appSettings, true);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedTransactions.SaveTransactions, result.SaveTransactions);
        }
    }
}

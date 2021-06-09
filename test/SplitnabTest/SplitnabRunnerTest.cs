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
        private readonly IGetYnabInfoOperation _getYnabInfoOperation;
        private readonly IYnabClient _ynabClient;

        private SplitnabRunner _sut;

        public SplitnabRunnerTest()
        {
            _logger = Substitute.For<ILogger<SplitnabRunner>>();
            _getSplitwiseInfoOperation = Substitute.For<IGetSplitwiseInfoOperation>();
            _getYnabInfoOperation = Substitute.For<IGetYnabInfoOperation>();
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
            var splitwiseInfo = new SplitwiseInfo
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
            _getSplitwiseInfoOperation.Invoke(appSettings).Returns(splitwiseInfo);

            var ynabAccountGuid = Guid.NewGuid();
            var ynabInfo = new YnabInfo
            {
                Budget = new BudgetSummary {Id = Guid.NewGuid()},
                SplitwiseAccount = new Account {Id = ynabAccountGuid}
            };
            _getYnabInfoOperation.Invoke(appSettings).Returns(ynabInfo);

            var expectedTransactions = new Transactions
            {
                SaveTransactions = new List<SaveTransaction>
                {
                    new()
                    {
                        AccountId = ynabAccountGuid,
                        Date = expenseDate,
                        Amount = 61725000,
                        PayeeName = "friendName",
                        Memo = "expensive",
                        Approved = false
                    }
                }
            };

            // Act
            _sut = new SplitnabRunner(_logger, _getSplitwiseInfoOperation, _getYnabInfoOperation, _ynabClient);
            var result = await _sut.Run(appSettings, true);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedTransactions.SaveTransactions, result.SaveTransactions);
        }
    }
}

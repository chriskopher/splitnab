using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Splitnab;
using Splitnab.Model;
using Xunit;
using YnabClient;
using YnabClient.Model.Accounts;
using YnabClient.Model.Budgets;

namespace SplitnabTest
{
    public class GetYnabInfoOperationTest
    {
        private readonly ILogger<GetYnabInfoOperation> _logger;
        private readonly IYnabClient _ynabClient;

        private GetYnabInfoOperation _sut;

        public GetYnabInfoOperationTest()
        {
            _logger = Substitute.For<ILogger<GetYnabInfoOperation>>();
            _ynabClient = Substitute.For<IYnabClient>();
        }

        [Fact]
        public async Task InvokeWithValidSettingsReturnsExpected()
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

            var ynabBudgetGuid = Guid.NewGuid();
            var ynabBudgets = new BudgetSummaryResponse
            {
                Data = new BudgetModel
                {
                    Budgets = new List<BudgetSummary> {new() {Name = "budgetName", Id = ynabBudgetGuid}}
                }
            };
            _ynabClient.GetBudgets(true).Returns(ynabBudgets);

            var ynabBudgetAccountGuid = Guid.NewGuid();
            var ynabBudgetAccounts = new AccountsResponse
            {
                Data = new AccountsModel
                {
                    Accounts = new List<Account>
                    {
                        new() {Name = "splitwiseAccountName", Id = ynabBudgetAccountGuid}
                    }
                }
            };
            _ynabClient.GetBudgetAccounts(ynabBudgetGuid).Returns(ynabBudgetAccounts);

            var expected = new YnabInfo
            {
                Budget = new BudgetSummary {Name = "budgetName", Id = ynabBudgetGuid},
                SplitwiseAccount = new Account {Name = "splitwiseAccountName", Id = ynabBudgetAccountGuid}
            };

            // Act
            _sut = new GetYnabInfoOperation(_logger, _ynabClient);
            var result = await _sut.Invoke(appSettings);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task InvokeWithNullBudgetsReturnsNull()
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

            var ynabBudgets = new BudgetSummaryResponse();
            _ynabClient.GetBudgets(true).Returns(ynabBudgets);

            // Act
            _sut = new GetYnabInfoOperation(_logger, _ynabClient);
            var result = await _sut.Invoke(appSettings);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task InvokeWithInvalidBudgetReturnsNull()
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
                    BudgetName = "invalidBudgetName",
                    SplitwiseAccountName = "splitwiseAccountName"
                }
            };

            var ynabBudgetGuid = Guid.NewGuid();
            var ynabBudgets = new BudgetSummaryResponse
            {
                Data = new BudgetModel
                {
                    Budgets = new List<BudgetSummary> {new() {Name = "budgetName", Id = ynabBudgetGuid}}
                }
            };
            _ynabClient.GetBudgets(true).Returns(ynabBudgets);

            // Act
            _sut = new GetYnabInfoOperation(_logger, _ynabClient);
            var result = await _sut.Invoke(appSettings);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task InvokeWithNullBudgetAccountsReturnsNull()
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

            var ynabBudgetGuid = Guid.NewGuid();
            var ynabBudgets = new BudgetSummaryResponse
            {
                Data = new BudgetModel
                {
                    Budgets = new List<BudgetSummary> {new() {Name = "budgetName", Id = ynabBudgetGuid}}
                }
            };
            _ynabClient.GetBudgets(true).Returns(ynabBudgets);

            var ynabBudgetAccounts = new AccountsResponse();
            _ynabClient.GetBudgetAccounts(ynabBudgetGuid).Returns(ynabBudgetAccounts);

            // Act
            _sut = new GetYnabInfoOperation(_logger, _ynabClient);
            var result = await _sut.Invoke(appSettings);

            // Assert
            Assert.Null(result);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Splitnab;
using SplitwiseClient;
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
        private readonly ISplitwiseClient _splitwiseClient;
        private readonly IYnabClient _ynabClient;

        private SplitnabRunner _sut;

        public SplitnabRunnerTest()
        {
            _logger = Substitute.For<ILogger<SplitnabRunner>>();
            _splitwiseClient = Substitute.For<ISplitwiseClient>();
            _ynabClient = Substitute.For<IYnabClient>();
        }

        [Fact]
        public async Task Run_WithCorrectValues_ReturnsExpected()
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

            var expectedUser = new CurrentUserResponse {User = new User {Id = 1}};
            _splitwiseClient.GetCurrentUser().Returns(expectedUser);

            var expectedFriends = new FriendsResponse
            {
                Friends = new List<FriendModel> {new() {Email = "friendEmail", FirstName = "firstName", Id = 123}}
            };
            _splitwiseClient.GetFriends().Returns(expectedFriends);

            var expectedExpenseDate = DateTime.Now.AddDays(1);
            var expectedExpenses = new ExpensesResponse
            {
                Expenses = new List<Expense>
                {
                    new()
                    {
                        Cost = "123.45",
                        CreationMethod = "payment",
                        Date = expectedExpenseDate,
                        Description = "expensive",
                        Repayments = new List<Repayment> {new() {From = 123}}
                    }
                }
            };
            _splitwiseClient
                .GetExpenses(friendId: 123, datedAfter: appSettings.Splitwise.TransactionsDatedAfter, limit: 0)
                .Returns(expectedExpenses);

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
                        Date = expectedExpenseDate,
                        Amount = 123450,
                        PayeeName = "firstName",
                        Memo = "expensive",
                        Approved = false
                    }
                }
            };

            // Act
            _sut = new SplitnabRunner(_logger, _splitwiseClient, _ynabClient);
            var result = await _sut.Run(appSettings, true);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedTransactions.SaveTransactions, result.SaveTransactions);
        }
    }
}

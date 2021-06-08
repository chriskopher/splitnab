using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Splitnab;
using Splitnab.Model;
using SplitwiseClient;
using SplitwiseClient.Model.Expenses;
using SplitwiseClient.Model.Friends;
using SplitwiseClient.Model.Users;
using Xunit;

namespace SplitnabTest
{
    public class GetSplitwiseInfoOperationTest
    {
        private readonly ILogger<GetSplitwiseInfoOperation> _logger;
        private readonly ISplitwiseClient _splitwiseClient;

        private GetSplitwiseInfoOperation _sut;

        public GetSplitwiseInfoOperationTest()
        {
            _logger = Substitute.For<ILogger<GetSplitwiseInfoOperation>>();
            _splitwiseClient = Substitute.For<ISplitwiseClient>();
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

            var expectedUser = new CurrentUserResponse {User = new User {Id = 1, FirstName = "firstName"}};
            _splitwiseClient.GetCurrentUser().Returns(expectedUser);

            var expectedFriends = new FriendsResponse
            {
                Friends = new List<FriendModel> {new() {Email = "friendEmail", FirstName = "friendName", Id = 2}}
            };
            _splitwiseClient.GetFriends().Returns(expectedFriends);

            var expectedExpenses = new ExpensesResponse {Expenses = new List<Expense> {new() {Cost = "123456"}}};
            _splitwiseClient
                .GetExpenses(friendId: 2, datedAfter: appSettings.Splitwise.TransactionsDatedAfter, limit: 0)
                .Returns(expectedExpenses);

            var expectedResult = new SplitwiseInfo
            {
                CurrentUser = new User {Id = 1, FirstName = "firstName"},
                Friend = new FriendModel {Id = 2, FirstName = "friendName", Email = "friendEmail"},
                Expenses = new List<Expense> {new() {Cost = "123456"}}
            };

            // Act
            _sut = new GetSplitwiseInfoOperation(_logger, _splitwiseClient);
            var result = await _sut.Invoke(appSettings);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResult.CurrentUser, result.CurrentUser);
            Assert.Equal(expectedResult.Friend, result.Friend);
            Assert.Equal(expectedResult.Expenses, result.Expenses);
        }

        [Fact]
        public async Task InvokeWithNullCurrentUserReturnsNull()
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

            var expectedUser = new CurrentUserResponse();
            _splitwiseClient.GetCurrentUser().Returns(expectedUser);

            // Act
            _sut = new GetSplitwiseInfoOperation(_logger, _splitwiseClient);
            var result = await _sut.Invoke(appSettings);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task InvokeWithNullFriendsReturnsNull()
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

            var expectedUser = new CurrentUserResponse();
            _splitwiseClient.GetCurrentUser().Returns(expectedUser);

            var expectedFriends = new FriendsResponse();
            _splitwiseClient.GetFriends().Returns(expectedFriends);

            // Act
            _sut = new GetSplitwiseInfoOperation(_logger, _splitwiseClient);
            var result = await _sut.Invoke(appSettings);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task InvokeWithInvalidFriendEmailReturnsNull()
        {
            // Arrange
            var appSettings = new AppSettings
            {
                Splitwise = new Splitwise
                {
                    ConsumerKey = "consumerKey",
                    ConsumerSecret = "consumerSecret",
                    FriendEmail = "invalidFriendEmail",
                    TransactionsDatedAfter = new DateTimeOffset()
                },
                Ynab = new Ynab
                {
                    PersonalAccessToken = "personalAccessToken",
                    BudgetName = "budgetName",
                    SplitwiseAccountName = "splitwiseAccountName"
                }
            };

            var expectedUser = new CurrentUserResponse {User = new User {Id = 1, FirstName = "firstName"}};
            _splitwiseClient.GetCurrentUser().Returns(expectedUser);

            var expectedFriends = new FriendsResponse
            {
                Friends = new List<FriendModel> {new() {Email = "friendEmail", FirstName = "friendName", Id = 2}}
            };
            _splitwiseClient.GetFriends().Returns(expectedFriends);

            // Act
            _sut = new GetSplitwiseInfoOperation(_logger, _splitwiseClient);
            var result = await _sut.Invoke(appSettings);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task InvokeWithNullExpensesReturnsExpected()
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

            var expectedUser = new CurrentUserResponse {User = new User {Id = 1, FirstName = "firstName"}};
            _splitwiseClient.GetCurrentUser().Returns(expectedUser);

            var expectedFriends = new FriendsResponse
            {
                Friends = new List<FriendModel> {new() {Email = "friendEmail", FirstName = "friendName", Id = 2}}
            };
            _splitwiseClient.GetFriends().Returns(expectedFriends);

            var expectedExpenses = new ExpensesResponse();
            _splitwiseClient
                .GetExpenses(friendId: 2, datedAfter: appSettings.Splitwise.TransactionsDatedAfter, limit: 0)
                .Returns(expectedExpenses);

            // Act
            _sut = new GetSplitwiseInfoOperation(_logger, _splitwiseClient);
            var result = await _sut.Invoke(appSettings);

            // Assert
            Assert.Null(result);
        }
    }
}

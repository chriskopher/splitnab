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

        private readonly AppSettings _appSettings = new()
        {
            Splitwise = new Splitwise
            {
                ConsumerKey = "consumerKey",
                ConsumerSecret = "consumerSecret",
                FriendEmail = "friendEmail",
                TransactionsDatedAfter = DateTimeOffset.Now
            },
            Ynab = new Ynab
            {
                PersonalAccessToken = "personalAccessToken",
                BudgetName = "budgetName",
                SplitwiseAccountName = "splitwiseAccountName"
            }
        };

        public SplitnabRunnerTest()
        {
            _logger = Substitute.For<ILogger<SplitnabRunner>>();
            _getSplitwiseInfoOperation = Substitute.For<IGetSplitwiseInfoOperation>();
            _getYnabInfoOperation = Substitute.For<IGetYnabInfoOperation>();
            _ynabClient = Substitute.For<IYnabClient>();
        }

        [Fact]
        public async Task RunWithCorrectValuesForSplitwiseLendingAndNoDryRunReturnsExpected()
        {
            // Arrange
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
            _getSplitwiseInfoOperation.Invoke(_appSettings).Returns(splitwiseInfo);

            var ynabAccountGuid = Guid.NewGuid();
            var ynabInfo = new YnabInfo
            {
                Budget = new BudgetSummary {Id = Guid.NewGuid()},
                SplitwiseAccount = new Account {Id = ynabAccountGuid}
            };
            _getYnabInfoOperation.Invoke(_appSettings).Returns(ynabInfo);

            var expectedTransactions = new Transactions
            {
                SaveTransactions = new List<SaveTransaction>
                {
                    new()
                    {
                        AccountId = ynabAccountGuid,
                        Date = expenseDate,
                        Amount = 61725000, // Should be half the expense cost, x1000 to get milli-units
                        PayeeName = "friendName",
                        Memo = "expensive",
                        Approved = false
                    }
                }
            };

            // Act
            _sut = new SplitnabRunner(_logger, _getSplitwiseInfoOperation, _getYnabInfoOperation, _ynabClient);
            var result = await _sut.Run(_appSettings, false);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedTransactions.SaveTransactions, result.SaveTransactions);
            _ynabClient.Received().ConfigureAuthorization(_appSettings.Ynab.PersonalAccessToken);
            await _ynabClient.Received().PostTransactions(ynabInfo.Budget.Id, result);
        }

        [Fact]
        public async Task RunWithCorrectValuesForSplitwiseLendingReturnsExpected()
        {
            // Arrange
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
            _getSplitwiseInfoOperation.Invoke(_appSettings).Returns(splitwiseInfo);

            var ynabAccountGuid = Guid.NewGuid();
            var ynabInfo = new YnabInfo
            {
                Budget = new BudgetSummary {Id = Guid.NewGuid()},
                SplitwiseAccount = new Account {Id = ynabAccountGuid}
            };
            _getYnabInfoOperation.Invoke(_appSettings).Returns(ynabInfo);

            var expectedTransactions = new Transactions
            {
                SaveTransactions = new List<SaveTransaction>
                {
                    new()
                    {
                        AccountId = ynabAccountGuid,
                        Date = expenseDate,
                        Amount = 61725000, // Should be half the expense cost, x1000 to get milli-units
                        PayeeName = "friendName",
                        Memo = "expensive",
                        Approved = false
                    }
                }
            };

            // Act
            _sut = new SplitnabRunner(_logger, _getSplitwiseInfoOperation, _getYnabInfoOperation, _ynabClient);
            var result = await _sut.Run(_appSettings, true);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedTransactions.SaveTransactions, result.SaveTransactions);
        }

        [Fact]
        public async Task RunWithCorrectValuesForSplitwiseLendingWithMultipleExpensesReturnsExpected()
        {
            // Arrange
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
                    },
                    new()
                    {
                        Cost = "22.22",
                        Date = expenseDate,
                        Description = "another one",
                        Repayments = new List<Repayment> {new() {From = 2}}
                    },
                    new()
                    {
                        Cost = "33.33",
                        Date = expenseDate.AddDays(1),
                        Description = "something else",
                        Repayments = new List<Repayment> {new() {From = 2}}
                    }
                }
            };
            _getSplitwiseInfoOperation.Invoke(_appSettings).Returns(splitwiseInfo);

            var ynabAccountGuid = Guid.NewGuid();
            var ynabInfo = new YnabInfo
            {
                Budget = new BudgetSummary {Id = Guid.NewGuid()},
                SplitwiseAccount = new Account {Id = ynabAccountGuid}
            };
            _getYnabInfoOperation.Invoke(_appSettings).Returns(ynabInfo);

            var expectedTransactions = new Transactions
            {
                SaveTransactions = new List<SaveTransaction>
                {
                    new()
                    {
                        AccountId = ynabAccountGuid,
                        Date = expenseDate,
                        Amount = 61725000, // Should be half the expense cost, x1000 to get milli-units
                        PayeeName = "friendName",
                        Memo = "expensive",
                        Approved = false
                    },
                    new()
                    {
                        AccountId = ynabAccountGuid,
                        Date = expenseDate,
                        Amount = 11110,
                        PayeeName = "friendName",
                        Memo = "another one",
                        Approved = false
                    },
                    new()
                    {
                        AccountId = ynabAccountGuid,
                        Date = expenseDate.AddDays(1),
                        Amount = 16665,
                        PayeeName = "friendName",
                        Memo = "something else",
                        Approved = false
                    }
                }
            };

            // Act
            _sut = new SplitnabRunner(_logger, _getSplitwiseInfoOperation, _getYnabInfoOperation, _ynabClient);
            var result = await _sut.Run(_appSettings, true);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedTransactions.SaveTransactions, result.SaveTransactions);
        }

        [Fact]
        public async Task RunWithCorrectValuesForSplitwiseLoaningReturnsExpected()
        {
            // Arrange
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
                        Repayments = new List<Repayment> {new() {From = 1}}
                    }
                }
            };
            _getSplitwiseInfoOperation.Invoke(_appSettings).Returns(splitwiseInfo);

            var ynabAccountGuid = Guid.NewGuid();
            var ynabInfo = new YnabInfo
            {
                Budget = new BudgetSummary {Id = Guid.NewGuid()},
                SplitwiseAccount = new Account {Id = ynabAccountGuid}
            };
            _getYnabInfoOperation.Invoke(_appSettings).Returns(ynabInfo);

            var expectedTransactions = new Transactions
            {
                SaveTransactions = new List<SaveTransaction>
                {
                    new()
                    {
                        AccountId = ynabAccountGuid,
                        Date = expenseDate,
                        Amount = -61725000, // Should be negative and half the expense cost, x1000 to get milli-units
                        PayeeName = "friendName",
                        Memo = "expensive",
                        Approved = false
                    }
                }
            };

            // Act
            _sut = new SplitnabRunner(_logger, _getSplitwiseInfoOperation, _getYnabInfoOperation, _ynabClient);
            var result = await _sut.Run(_appSettings, true);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedTransactions.SaveTransactions, result.SaveTransactions);
        }

        [Fact]
        public async Task RunWithCorrectValuesForSplitwiseRepaymentReturnsExpected()
        {
            // Arrange
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
                        Repayments = new List<Repayment> {new() {From = 2}},
                        CreationMethod = "payment"
                    }
                }
            };
            _getSplitwiseInfoOperation.Invoke(_appSettings).Returns(splitwiseInfo);

            var ynabAccountGuid = Guid.NewGuid();
            var ynabInfo = new YnabInfo
            {
                Budget = new BudgetSummary {Id = Guid.NewGuid()},
                SplitwiseAccount = new Account {Id = ynabAccountGuid}
            };
            _getYnabInfoOperation.Invoke(_appSettings).Returns(ynabInfo);

            var expectedTransactions = new Transactions
            {
                SaveTransactions = new List<SaveTransaction>
                {
                    new()
                    {
                        AccountId = ynabAccountGuid,
                        Date = expenseDate,
                        Amount = 123450000, // Should be expense cost x1000 to get milli-units for the API
                        PayeeName = "friendName",
                        Memo = "expensive",
                        Approved = false
                    }
                }
            };

            // Act
            _sut = new SplitnabRunner(_logger, _getSplitwiseInfoOperation, _getYnabInfoOperation, _ynabClient);
            var result = await _sut.Run(_appSettings, true);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedTransactions.SaveTransactions, result.SaveTransactions);
        }

        [Fact]
        public async Task RunWithInvalidSplitwiseInfoReturnsNull()
        {
            // Arrange
            _getSplitwiseInfoOperation.Invoke(_appSettings).Returns((SplitwiseInfo)null);

            // Act
            _sut = new SplitnabRunner(_logger, _getSplitwiseInfoOperation, _getYnabInfoOperation, _ynabClient);
            var result = await _sut.Run(_appSettings, true);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task RunWithInvalidYnabInfoReturnsNull()
        {
            // Arrange
            var splitwiseInfo = new SplitwiseInfo();
            _getSplitwiseInfoOperation.Invoke(_appSettings).Returns(splitwiseInfo);

            _getYnabInfoOperation.Invoke(_appSettings).Returns((YnabInfo)null);

            // Act
            _sut = new SplitnabRunner(_logger, _getSplitwiseInfoOperation, _getYnabInfoOperation, _ynabClient);
            var result = await _sut.Run(_appSettings, true);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task RunWithExpensePaymentsNullReturnsZeroCostTransaction()
        {
            // Arrange
            var expenseDate = DateTime.Now;
            var splitwiseInfo = new SplitwiseInfo
            {
                CurrentUser = new User {Id = 1, FirstName = "firstName"},
                Friend = new FriendModel {Id = 2, FirstName = "friendName", Email = "friendEmail"},
                Expenses = new List<Expense>
                {
                    new() {Cost = "123450", Date = expenseDate, Description = "expensive", Repayments = null}
                }
            };
            _getSplitwiseInfoOperation.Invoke(_appSettings).Returns(splitwiseInfo);

            var ynabAccountGuid = Guid.NewGuid();
            var ynabInfo = new YnabInfo
            {
                Budget = new BudgetSummary {Id = Guid.NewGuid()},
                SplitwiseAccount = new Account {Id = ynabAccountGuid}
            };
            _getYnabInfoOperation.Invoke(_appSettings).Returns(ynabInfo);

            var expectedTransactions = new Transactions
            {
                SaveTransactions = new List<SaveTransaction>
                {
                    new()
                    {
                        AccountId = ynabAccountGuid,
                        Date = expenseDate,
                        Amount = 0, // Should be 0 because the expense repayments is null
                        PayeeName = "friendName",
                        Memo = "expensive",
                        Approved = false
                    }
                }
            };

            // Act
            _sut = new SplitnabRunner(_logger, _getSplitwiseInfoOperation, _getYnabInfoOperation, _ynabClient);
            var result = await _sut.Run(_appSettings, true);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedTransactions.SaveTransactions, result.SaveTransactions);
        }
    }
}

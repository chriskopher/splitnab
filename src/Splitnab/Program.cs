using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using SplitwiseClient;
using SplitwiseClient.Model.Expenses;
using YnabClient.Model.Transactions;

namespace Splitnab
{
    internal static class Program
    {
        public static async Task Main()
        {
            var settings = await ParseAppSettings();

            // 1. Get desired Splitwise expenses
            var splitwiseClient = new Client(settings.SwConsumerKey, settings.SwConsumerSecret);
            await splitwiseClient.ConfigureAccessToken();

            var currentUser = await splitwiseClient.GetCurrentUser();
            if (currentUser.User == null)
            {
                Console.WriteLine("Unable to fetch the current Splitwise user. Exiting...");
                return;
            }

            var friends = await splitwiseClient.GetFriends();
            if (friends.Friends == null)
            {
                Console.WriteLine("Unable to fetch current user's Splitwise friends list. Exiting...");
                return;
            }

            var friend = friends.Friends.FirstOrDefault(x => x.Email == settings.SwFriendEmail);
            if (friend == null)
            {
                Console.WriteLine("Unable to find the specified Splitwise friend. Exiting...");
                return;
            }

            var expenses = await splitwiseClient.GetExpenses(
                friendId: friend.Id,
                datedAfter: settings.SwTransactionsDatedAfter,
                limit: 0);
            if (expenses.Expenses == null)
            {
                Console.WriteLine("No Splitwise expenses found for the specified user and date range. Exiting...");
                return;
            }

            // 2. Map them as YNAB transactions
            var ynabClient = new YnabClient.Client(settings.YnabPersonalAccessToken);

            var ynabBudgets = await ynabClient.GetBudgets(true);
            if (ynabBudgets.Data?.Budgets == null)
            {
                Console.WriteLine("Unable to fetch YNAB budgets. Exiting...");
                return;
            }

            var ynabBudget = ynabBudgets.Data.Budgets.FirstOrDefault(x => x.Name == settings.YnabBudgetName);
            if (ynabBudget == null)
            {
                Console.Write("Unable to find YNAB budget. Exiting...");
                return;
            }

            var ynabBudgetAccounts = await ynabClient.GetBudgetAccounts(ynabBudget.Id);
            var splitwiseAccount = ynabBudgetAccounts.Data?.Accounts?
                .SingleOrDefault(x => x.Name == settings.YnabSplitwiseBudgetName);
            if (splitwiseAccount == null)
            {
                Console.WriteLine(
                    $"Unable to find YNAB splitwise account {settings.YnabBudgetName} in budget {settings.YnabBudgetName}. Exiting...");
                return;
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
            await ynabClient.PostTransactions(ynabBudget.Id, transactionsToPost);
            Console.WriteLine(
                $"Successfully saved {transactionsToPost.SaveTransactions.Count} transactions from Splitwise to YNAB using budget {settings.YnabBudgetName} in the {settings.YnabSplitwiseBudgetName} account!");
        }

        private static async Task<AppSettings> ParseAppSettings()
        {
            var serializerOptions = new JsonSerializerOptions {Converters = {new DynamicJsonConverter()}};
            var appSettings = await File.ReadAllTextAsync("appsettings.json");
            var json = JsonSerializer.Deserialize<dynamic>(appSettings, serializerOptions);

            return new AppSettings(json);
        }

        private static long CalculateYnabAmount(Expense expense, int currentUserId)
        {
            if (expense.Repayments == null)
            {
                Console.WriteLine(
                    $"Malformed Splitwise expense. Setting cost to 0 for {expense.Description} on {expense.Date}");
                return 0;
            }

            var cost = Convert.ToDouble(expense.Cost);
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

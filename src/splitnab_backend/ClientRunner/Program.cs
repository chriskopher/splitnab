using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using YnabClient;

namespace ClientRunner
{
    internal class Program
    {
        public static async Task Main()
        {
            var serializerOptions = new JsonSerializerOptions {Converters = {new DynamicJsonConverter()}};
            var appSettings = await File.ReadAllTextAsync("appsettings.json");
            var json = JsonSerializer.Deserialize<dynamic>(appSettings, serializerOptions);

            // var consumerKey = json.Splitwise.ConsumerKey;
            // var consumerSecret = json.Splitwise.ConsumerSecret;
            //
            // var client = new SplitwiseClient.Client(consumerKey, consumerSecret);
            // await client.ConfigureAccessToken();
            //
            // var currentUserResponse = await client.GetCurrentUser();
            // var userId = await client.GetUser(7789945);
            // var friends = await client.GetFriends();
            // var expenses = await client.GetExpenses(friendId: 24097313, limit: 25);
            //
            // Console.WriteLine(currentUserResponse?.User?.Id);
            // Console.WriteLine(userId?.User?.Id);
            // Console.WriteLine(friends?.Friends?.Count);
            // Console.WriteLine(expenses?.Expenses?.Count);

            var personalAccessToken = json.YNAB.PersonalAccessToken;
            var ynabClient = new Client(personalAccessToken);

            var ynabUser = await ynabClient.GetCurrentUser();
            var ynabBudgets = await ynabClient.GetBudgets(true);
            var budgetId = ynabBudgets?.Data?.Budgets?[1].Id;
            if (budgetId.HasValue)
            {
                var ynabBudgetAccounts = await ynabClient.GetBudgetAccounts(budgetId.Value);
                var ynabBudgetCategories = await ynabClient.GetBudgetCategories(budgetId.Value);
                var ynabBudgetPayees = await ynabClient.GetBudgetPayees(budgetId.Value);
            }

            Console.WriteLine(ynabUser?.Data?.User?.Id);
        }
    }
}

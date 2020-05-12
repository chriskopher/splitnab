using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace SplitwiseClientRunner
{
    class Program
    {
        public static async Task Main()
        {
            var serializerOptions = new JsonSerializerOptions {Converters = {new DynamicJsonConverter()}};
            var appSettings = await File.ReadAllTextAsync("appsettings.json");
            var json = JsonSerializer.Deserialize<dynamic>(appSettings, serializerOptions);

            var consumerKey = json.Splitwise.ConsumerKey;
            var consumerSecret = json.Splitwise.ConsumerSecret;

            var client = new SplitwiseClient.Client(consumerKey, consumerSecret);
            await client.ConfigureAccessToken();

            var currentUser = await client.GetCurrentUser();
            var userId = await client.GetUser(7789945);

            Console.WriteLine(currentUser.Info.Id);
        }
    }
}

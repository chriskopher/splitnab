﻿using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using SplitwiseClient;

namespace SplitwiseClientRunner
{
    internal class Program
    {
        public static async Task Main()
        {
            var serializerOptions = new JsonSerializerOptions {Converters = {new DynamicJsonConverter()}};
            var appSettings = await File.ReadAllTextAsync("appsettings.json");
            var json = JsonSerializer.Deserialize<dynamic>(appSettings, serializerOptions);

            var consumerKey = json.Splitwise.ConsumerKey;
            var consumerSecret = json.Splitwise.ConsumerSecret;

            var client = new Client(consumerKey, consumerSecret);
            await client.ConfigureAccessToken();

            var currentUserResponse = await client.GetCurrentUser();
            var userId = await client.GetUser(7789945);
            var friends = await client.GetFriends();
            var expenses = await client.GetExpenses(friendId: 24097313, limit: 25);

            Console.WriteLine(currentUserResponse?.User?.Id);
            Console.WriteLine(userId?.User?.Id);
            Console.WriteLine(friends?.Friends?.Count);
            Console.WriteLine(expenses?.Expenses?.Count);
        }
    }
}
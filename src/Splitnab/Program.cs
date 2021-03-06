﻿using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RestSharp;
using Splitnab.Model;
using SplitwiseClient;
using YnabClient;

namespace Splitnab
{
    public static class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var rootCommand =
                new RootCommand("A program to import Splitwise transactions to YNAB.")
                {
                    new Option<string>(
                        "--appsettings-file",
                        () => "appsettings.json",
                        "The location of the appsettings.json file, relative to program execution directory."),
                    new Option<bool>(
                        "--dry-run",
                        "Option to run splitnab without saving the transactions.")
                };

            rootCommand.Handler = CommandHandler.Create<string, bool>(
                async (appsettingsFile, dryRun) =>
                {
                    using var host = CreateHostBuilder().Build();
                    var splitnabRunner = ActivatorUtilities.CreateInstance<SplitnabRunner>(host.Services);

                    var appSettings = await ParseAppSettings(appsettingsFile);

                    var transactions = await splitnabRunner.Run(appSettings, dryRun);
                    var success = transactions?.SaveTransactions.Any();

                    if (!dryRun && success.HasValue)
                    {
                        // After a successful run, save the current date to the appsettings file
                        // so that the next run won't duplicate the already imported transactions
                        await WriteCurrentDate(appsettingsFile, appSettings);
                    }
                });

            return await new CommandLineBuilder(rootCommand)
                .UseDefaults()
                .Build()
                .InvokeAsync(args);
        }

        private static IHostBuilder CreateHostBuilder() =>
            Host.CreateDefaultBuilder()
                .ConfigureLogging(logging =>
                {
                    logging.AddSimpleConsole();
                })
                .ConfigureServices((_, services) =>
                    services.AddTransient<IRestClient, RestClient>()
                        .AddSingleton<ISplitwiseClient, SplitwiseClient.Client>()
                        .AddSingleton<IYnabClient, YnabClient.Client>()
                        .AddSingleton<IGetSplitwiseInfoOperation, GetSplitwiseInfoOperation>()
                        .AddSingleton<IGetYnabInfoOperation, GetYnabInfoOperation>()
                        .AddSingleton<SplitnabRunner>());

        private static async Task<AppSettings> ParseAppSettings(string appsettingsFile)
        {
            await using var fileStream = File.OpenRead(appsettingsFile);

            var options = new JsonSerializerOptions {PropertyNameCaseInsensitive = true};

            return await JsonSerializer.DeserializeAsync<AppSettings>(fileStream, options)
                   ?? throw new ArgumentException(
                       "Provided appsettings.json file deserialized to null. Ensure this file has the correct format.");
        }

        private static async Task WriteCurrentDate(string filePath, AppSettings appSettings)
        {
            appSettings.Splitwise.TransactionsDatedAfter = DateTimeOffset.Now;

            var options = new JsonSerializerOptions {WriteIndented = true};
            var jsonString = JsonSerializer.Serialize(appSettings, options);

            await File.WriteAllTextAsync(filePath, jsonString);
        }
    }
}

using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
                    var splitnab = ActivatorUtilities.CreateInstance<Splitnab>(host.Services);

                    await splitnab.Run(await ParseAppSettings(appsettingsFile), dryRun);
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
                    services.AddSingleton<ISplitwiseClient, SplitwiseClient.Client>()
                        .AddSingleton<IYnabClient, YnabClient.Client>()
                        .AddSingleton<Splitnab>());

        private static async Task<AppSettings> ParseAppSettings(string appsettingsFile)
        {
            var serializerOptions = new JsonSerializerOptions {Converters = {new DynamicJsonConverter()}};
            var appSettings = await File.ReadAllTextAsync(appsettingsFile);
            var json = JsonSerializer.Deserialize<dynamic>(appSettings, serializerOptions);

            return new AppSettings(json);
        }
    }
}

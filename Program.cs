using System;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using NuciCLI;
using NuciCLI.Arguments;
using NuciLog;
using NuciLog.Configuration;
using NuciLog.Core;
using NuciWeb;

using OpenQA.Selenium;

using WirelessRouterRebooter.Configuration;
using WirelessRouterRebooter.Logging;
using WirelessRouterRebooter.Service;
using WirelessRouterRebooter.Service.Models;
using WirelessRouterRebooter.Service.Processors;

namespace WirelessRouterRebooter
{
    public sealed class Program
    {
        static BotSettings botSettings;
        static DebugSettings debugSettings;
        static NuciLoggerSettings loggingSettings;
        static IWebDriver webDriver;

        static ILogger logger;

        static void Main(string[] args)
        {
            botSettings = new BotSettings();
            debugSettings = new DebugSettings();
            loggingSettings = new NuciLoggerSettings();

            IConfiguration config = LoadConfiguration();
            config.Bind(nameof(BotSettings), botSettings);
            config.Bind(nameof(DebugSettings), debugSettings);
            config.Bind(nameof(NuciLoggerSettings), loggingSettings);

            webDriver = WebDriverInitialiser.InitialiseAvailableWebDriver(debugSettings.IsDebugMode);

            IServiceProvider serviceProvider = new ServiceCollection()
                .AddSingleton(botSettings)
                .AddSingleton(debugSettings)
                .AddSingleton(loggingSettings)
                .AddSingleton<ILogger, NuciLogger>()
                .AddSingleton(webDriver)
                .AddTransient<IWebProcessor, WebProcessor>()
                .AddSingleton<IRouterProcessor, CompalCH7465VF>()
                .AddSingleton<IBotService, BotService>()
                .BuildServiceProvider();

            logger = serviceProvider.GetService<ILogger>();
            logger.Info(Operation.StartUp, $"Application started");

            UserCredentials userCredentials = RetrieveCredentials(args);

            Run(serviceProvider, userCredentials);
        }

        static UserCredentials RetrieveCredentials(string[] args)
        {
            logger.Info(
                MyOperation.ParseArguments,
                OperationStatus.Started,
                "Parsing the command line arguments");

            UserCredentials credentials;

            try
            {
                ArgumentParser argumentsParser = new();
                argumentsParser.AddArgument("username", "The username for the router login", false, "admin");
                argumentsParser.AddArgument("password", "The password for the router login", false, "admin");

                ArgumentsCollection arguments = argumentsParser.ParseArgs(args);

                credentials = new()
                {
                    Username = arguments.Get<string>("username"),
                    Password = arguments.Get<string>("password")
                };
            }
            catch (Exception ex)
            {
                logger.Error(
                    MyOperation.ParseArguments,
                    OperationStatus.Failure,
                    "Failed to parse the command line arguments",
                    ex);

                throw;
            }

            logger.Debug(
                MyOperation.ParseArguments,
                OperationStatus.Success,
                "Finished parsing the command line arguments",
                new LogInfo(MyLogInfoKey.Username, credentials.Username));

            return credentials;
        }

        static void Run(IServiceProvider serviceProvider, UserCredentials userCredentials)
            {
                IBotService botService = serviceProvider.GetService<IBotService>();

                try
                {
                    botService.Run(userCredentials);
                }
                catch (AggregateException ex)
                {
                    foreach (Exception innerException in ex.InnerExceptions)
                    {
                        logger.Fatal(Operation.Unknown, OperationStatus.Failure, innerException);
                    }
                }
                catch (Exception ex)
                {
                    logger.Fatal(Operation.Unknown, OperationStatus.Failure, ex);
                }

                webDriver.Quit();
            }

        static IConfiguration LoadConfiguration() => new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true, true)
            .Build();
    }
}

using System;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using NuciCLI.Arguments;
using NuciLog;
using NuciLog.Configuration;
using NuciLog.Core;
using NuciWeb.Automation;
using NuciWeb.Automation.Selenium;
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

            ArgumentsCollection arguments = ParseArguments(args);
            string deviceKey = ParseDeviceArgument(arguments);

            webDriver = WebDriverInitialiser.InitialiseAvailableWebDriver(debugSettings.IsDebugMode);

            IServiceProvider serviceProvider = new ServiceCollection()
                .AddSingleton(botSettings)
                .AddSingleton(debugSettings)
                .AddSingleton(loggingSettings)
                .AddSingleton<ILogger, NuciLogger>()
                .AddSingleton(webDriver)
                .AddTransient<IWebProcessor, SeleniumWebProcessor>()
                .AddKeyedSingleton<IRouterProcessor, CompalCH7465VF>("ch7465vf")
                .AddKeyedSingleton<IRouterProcessor, ZteF660>("f660")
                .AddSingleton(sp => sp.GetRequiredKeyedService<IRouterProcessor>(deviceKey))
                .AddSingleton<IBotService, BotService>()
                .BuildServiceProvider();

            logger = serviceProvider.GetService<ILogger>();
            logger.Info(Operation.StartUp, $"Application started");

            UserCredentials userCredentials = RetrieveCredentials(arguments);

            Run(serviceProvider, userCredentials);
        }

        static ArgumentsCollection ParseArguments(string[] args)
        {
            ArgumentParser argumentsParser = new();
            argumentsParser.AddArgument("username", "The username for the router login", false, "admin");
            argumentsParser.AddArgument("password", "The password for the router login", false, "admin");
            argumentsParser.AddArgument("device", "The router device model (ch7465vf, f660)", false, "ch7465vf");

            return argumentsParser.ParseArgs(args);
        }

        static string ParseDeviceArgument(ArgumentsCollection arguments)
        {
            string device = arguments.Get<string>("device").ToLowerInvariant();

            if (device != "ch7465vf" && device != "f660")
            {
                throw new ArgumentException($"Unknown device '{device}'. Valid values are: ch7465vf, f660");
            }

            return device;
        }

        static UserCredentials RetrieveCredentials(ArgumentsCollection arguments)
        {
            logger.Info(
                MyOperation.ParseArguments,
                OperationStatus.Started,
                "Parsing the command line arguments");

            UserCredentials credentials;

            try
            {
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

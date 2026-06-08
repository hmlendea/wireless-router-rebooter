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
            RouterAccessInfo accessInfo = RetrieveAccessInfo(arguments);
            string deviceKey = ParseDeviceArgument(arguments);

            webDriver = WebDriverInitialiser.InitialiseAvailableWebDriver(debugSettings.IsDebugMode);

            IServiceProvider serviceProvider = new ServiceCollection()
                .AddSingleton(botSettings)
                .AddSingleton(debugSettings)
                .AddSingleton(loggingSettings)
                .AddSingleton(accessInfo)
                .AddSingleton<ILogger, NuciLogger>()
                .AddSingleton(webDriver)
                .AddTransient<IWebProcessor, SeleniumWebProcessor>()
                .AddKeyedSingleton<IRouterProcessor, CompalCH7465VF>("ch7465vf")
                .AddKeyedSingleton<IRouterProcessor, TpLinkMR105Processor>("tl-mr105")
                .AddKeyedSingleton<IRouterProcessor, ZteF660>("f660")
                .AddSingleton(sp => sp.GetRequiredKeyedService<IRouterProcessor>(deviceKey))
                .AddSingleton<IBotService, BotService>()
                .BuildServiceProvider();

            logger = serviceProvider.GetService<ILogger>();
            logger.Info(Operation.StartUp, $"Application started");

            Run(serviceProvider, accessInfo);
        }

        static ArgumentsCollection ParseArguments(string[] args)
        {
            ArgumentParser argumentsParser = new();
            argumentsParser.AddArgument("username", "The username for the router login", false, "admin");
            argumentsParser.AddArgument("password", "The password for the router login", false, "admin");
            argumentsParser.AddArgument("ip", "The custom router IP address", false, "");
            argumentsParser.AddArgument("router", "The router model (ch7465vf, f660, tl-mr105)", false, "ch7465vf");

            return argumentsParser.ParseArgs(args);
        }

        static string ParseDeviceArgument(ArgumentsCollection arguments)
        {
            string device = arguments.Get<string>("router").ToLowerInvariant();

            if (device != "ch7465vf" &&
                device != "f660" &&
                device != "tl-mr105")
            {
                throw new ArgumentException($"Unknown device '{device}'. Valid values are: ch7465vf, f660, tl-mr105");
            }

            return device;
        }

        static RouterAccessInfo RetrieveAccessInfo(ArgumentsCollection arguments)
        {
            return new RouterAccessInfo
            {
                Username = arguments.Get<string>("username"),
                Password = arguments.Get<string>("password"),
                IpAddress = arguments.Get<string>("ip")
            };
        }

        static void Run(IServiceProvider serviceProvider, RouterAccessInfo accessInfo)
            {
                IBotService botService = serviceProvider.GetService<IBotService>();

                try
                {
                    botService.Run(accessInfo);
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

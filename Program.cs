using System;
using System.IO;
using System.Threading;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using NuciCLI;
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
        static readonly string[] UsernameOptions = ["-u", "--user", "--usr",  "--username"];
        static readonly string[] PasswordOptions = ["-p", "--pass", "--pwd", "--password"];

        static TimeSpan RetryDelay => TimeSpan.FromMinutes(5);

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

            UserCredentials userCredentials = new()
            {
                Username = CliArgumentsReader.GetOptionValue(args, UsernameOptions),
                Password = CliArgumentsReader.GetOptionValue(args, PasswordOptions)
            };

            Run(serviceProvider, userCredentials);
        }

        static void Run(IServiceProvider serviceProvider, UserCredentials userCredentials)
        {
            IBotService botService = serviceProvider.GetService<IBotService>();
            bool routerWasRebooted = false;

            while (!routerWasRebooted)
            {
                try
                {
                    botService.Run(userCredentials);
                    routerWasRebooted = true;
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

                SaveCrashScreenshot();

                logger.Info(
                    MyOperation.CrashRecovery,
                    new LogInfo(MyLogInfoKey.RetryDelay, RetryDelay.TotalMilliseconds));

                Thread.Sleep((int)RetryDelay.TotalMilliseconds);
            }
        }

        static IConfiguration LoadConfiguration() => new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true, true)
            .Build();

        static void SaveCrashScreenshot()
        {
            if (!debugSettings.IsCrashScreenshotEnabled)
            {
                return;
            }

            string directory = Path.GetDirectoryName(loggingSettings.LogFilePath);
            string filePath = Path.Combine(directory, debugSettings.CrashScreenshotFileName);

            ((ITakesScreenshot)webDriver)
                .GetScreenshot()
                .SaveAsFile(filePath);
        }
    }
}

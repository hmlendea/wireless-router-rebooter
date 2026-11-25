using System.Collections.Generic;
using System.IO;

using Microsoft.Extensions.Configuration;

using NUnit.Framework;

using NuciLog.Configuration;

using WirelessRouterRebooter.Configuration;

namespace WirelessRouterRebooter.IntegrationTests.Configuration
{
    [TestFixture]
    public sealed class ConfigurationIntegrationTests
    {
        [Test]
        public void GivenApplicationSettings_WhenBindingConfiguration_ThenAllSettingsAreLoaded()
        {
            using MemoryStream stream = new();
            using StreamWriter writer = new(stream, leaveOpen: true);
            writer.Write("{\"botSettings\":{\"pageLoadTimeout\":96},\"debugSettings\":{\"crashScreenshotFileName\":\"crash.png\",\"isDebugMode\":true},\"nuciLoggerSettings\":{\"logFilePath\":\"./logfile.log\",\"minimumLevel\":\"Info\",\"isFileOutputEnabled\":true}}");
            writer.Flush();
            stream.Position = 0;

            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonStream(stream)
                .Build();
            BotSettings botSettings = new();
            DebugSettings debugSettings = new();
            NuciLoggerSettings loggerSettings = new();
            configuration.Bind(nameof(BotSettings), botSettings);
            configuration.Bind(nameof(DebugSettings), debugSettings);
            configuration.Bind(nameof(NuciLoggerSettings), loggerSettings);

            Assert.That(botSettings.PageLoadTimeout, Is.EqualTo(96));
            Assert.That(debugSettings.IsDebugMode, Is.True);
            Assert.That(debugSettings.IsCrashScreenshotEnabled, Is.True);
            Assert.That(debugSettings.CrashScreenshotFileName, Is.EqualTo("crash.png"));
            Assert.That(loggerSettings.LogFilePath, Is.EqualTo("./logfile.log"));
            Assert.That(loggerSettings.IsFileOutputEnabled, Is.True);
        }

        [Test]
        public void GivenMissingOptionalConfiguration_WhenBindingConfiguration_ThenSettingsKeepDefaults()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>())
                .Build();
            BotSettings botSettings = new();
            DebugSettings debugSettings = new();

            configuration.Bind(nameof(BotSettings), botSettings);
            configuration.Bind(nameof(DebugSettings), debugSettings);

            Assert.That(botSettings.PageLoadTimeout, Is.EqualTo(0));
            Assert.That(debugSettings.IsDebugMode, Is.False);
            Assert.That(debugSettings.IsCrashScreenshotEnabled, Is.False);
        }
    }
}

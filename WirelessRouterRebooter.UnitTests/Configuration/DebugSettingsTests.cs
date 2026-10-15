using System;
using NUnit.Framework;
using WirelessRouterRebooter.Configuration;

namespace WirelessRouterRebooter.UnitTests.Configuration
{
    public sealed class DebugSettingsTests
    {
        [Test]
        public void CrashScreenshotFileName_DefaultIsNull()
        {
            var settings = new DebugSettings();
            Assert.That(settings.CrashScreenshotFileName, Is.Null);
        }

        [Test]
        public void CrashScreenshotFileName_CanBeSetAndRetrieved()
        {
            var settings = new DebugSettings { CrashScreenshotFileName = "crash.png" };
            Assert.That(settings.CrashScreenshotFileName, Is.EqualTo("crash.png"));
        }

        [Test]
        public void IsDebugMode_DefaultIsFalse()
        {
            var settings = new DebugSettings();
            Assert.That(settings.IsDebugMode, Is.False);
        }

        [Test]
        public void IsDebugMode_CanBeSetAndRetrieved()
        {
            var settings = new DebugSettings { IsDebugMode = true };
            Assert.That(settings.IsDebugMode, Is.True);
        }

        [Test]
        public void IsCrashScreenshotEnabled_ReturnsFalse_WhenFileNameIsNull()
        {
            var settings = new DebugSettings { CrashScreenshotFileName = null };
            Assert.That(settings.IsCrashScreenshotEnabled, Is.False);
        }

        [Test]
        public void IsCrashScreenshotEnabled_ReturnsFalse_WhenFileNameIsEmpty()
        {
            var settings = new DebugSettings { CrashScreenshotFileName = "" };
            Assert.That(settings.IsCrashScreenshotEnabled, Is.False);
        }

        [Test]
        public void IsCrashScreenshotEnabled_ReturnsFalse_WhenFileNameIsWhitespace()
        {
            var settings = new DebugSettings { CrashScreenshotFileName = "   " };
            Assert.That(settings.IsCrashScreenshotEnabled, Is.False);
        }

        [Test]
        public void IsCrashScreenshotEnabled_ReturnsTrue_WhenFileNameIsSet()
        {
            var settings = new DebugSettings { CrashScreenshotFileName = "crash.png" };
            Assert.That(settings.IsCrashScreenshotEnabled, Is.True);
        }
    }
}
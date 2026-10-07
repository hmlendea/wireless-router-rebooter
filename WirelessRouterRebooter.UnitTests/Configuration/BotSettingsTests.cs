using System;
using NUnit.Framework;
using WirelessRouterRebooter.Configuration;

namespace WirelessRouterRebooter.UnitTests.Configuration
{
    public sealed class BotSettingsTests
    {
        [Test]
        public void PageLoadTimeout_DefaultIsZero()
        {
            var settings = new BotSettings();
            Assert.That(settings.PageLoadTimeout, Is.EqualTo(0));
        }

        [Test]
        public void PageLoadTimeout_CanBeSetAndRetrieved()
        {
            var settings = new BotSettings { PageLoadTimeout = 30 };
            Assert.That(settings.PageLoadTimeout, Is.EqualTo(30));
        }

        [Test]
        public void PageLoadTimeout_CanBeNegative()
        {
            var settings = new BotSettings { PageLoadTimeout = -1 };
            Assert.That(settings.PageLoadTimeout, Is.EqualTo(-1));
        }

        [Test]
        public void PageLoadTimeout_CanBeMaxInt()
        {
            var settings = new BotSettings { PageLoadTimeout = int.MaxValue };
            Assert.That(settings.PageLoadTimeout, Is.EqualTo(int.MaxValue));
        }
    }
}
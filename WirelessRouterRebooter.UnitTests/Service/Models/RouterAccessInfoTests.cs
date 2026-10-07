using System;
using NUnit.Framework;
using WirelessRouterRebooter.Service.Models;

namespace WirelessRouterRebooter.UnitTests.Service.Models
{
    public sealed class RouterAccessInfoTests
    {
        [Test]
        public void GetIpAddressOrDefault_ReturnsDefault_WhenIpAddressIsNull()
        {
            var info = new RouterAccessInfo { IpAddress = null };
            Assert.That(info.GetIpAddressOrDefault("192.168.1.1"), Is.EqualTo("192.168.1.1"));
        }

        [Test]
        public void GetIpAddressOrDefault_ReturnsDefault_WhenIpAddressIsEmpty()
        {
            var info = new RouterAccessInfo { IpAddress = "" };
            Assert.That(info.GetIpAddressOrDefault("192.168.1.1"), Is.EqualTo("192.168.1.1"));
        }

        [Test]
        public void GetIpAddressOrDefault_ReturnsDefault_WhenIpAddressIsWhitespace()
        {
            var info = new RouterAccessInfo { IpAddress = "   " };
            Assert.That(info.GetIpAddressOrDefault("192.168.1.1"), Is.EqualTo("192.168.1.1"));
        }

        [Test]
        public void GetIpAddressOrDefault_ReturnsIpAddress_WhenIpAddressIsSet()
        {
            var info = new RouterAccessInfo { IpAddress = "10.0.0.1" };
            Assert.That(info.GetIpAddressOrDefault("192.168.1.1"), Is.EqualTo("10.0.0.1"));
        }

        [Test]
        public void GetIpAddressOrDefault_ReturnsIpAddress_WhenIpAddressHasWhitespace()
        {
            var info = new RouterAccessInfo { IpAddress = "  10.0.0.1  " };
            Assert.That(info.GetIpAddressOrDefault("192.168.1.1"), Is.EqualTo("  10.0.0.1  "));
        }

        [Test]
        public void Username_CanBeSetAndRetrieved()
        {
            var info = new RouterAccessInfo { Username = "admin" };
            Assert.That(info.Username, Is.EqualTo("admin"));
        }

        [Test]
        public void Password_CanBeSetAndRetrieved()
        {
            var info = new RouterAccessInfo { Password = "secret" };
            Assert.That(info.Password, Is.EqualTo("secret"));
        }

        [Test]
        public void IpAddress_CanBeSetAndRetrieved()
        {
            var info = new RouterAccessInfo { IpAddress = "192.168.0.1" };
            Assert.That(info.IpAddress, Is.EqualTo("192.168.0.1"));
        }

        [Test]
        public void DefaultConstructor_CreatesInstanceWithNullProperties()
        {
            var info = new RouterAccessInfo();
            Assert.That(info.Username, Is.Null);
            Assert.That(info.Password, Is.Null);
            Assert.That(info.IpAddress, Is.Null);
        }
    }
}
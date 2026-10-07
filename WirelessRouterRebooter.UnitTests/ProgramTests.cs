using System;
using System.Collections.Generic;
using NUnit.Framework;
using NuciCLI.Arguments;
using WirelessRouterRebooter.Service.Models;

namespace WirelessRouterRebooter.UnitTests
{
    public sealed class ProgramArgumentParsingTests
    {
        [Test]
        public void ParseArguments_WithNoArgs_ReturnsDefaults()
        {
            var args = Array.Empty<string>();
            var parser = new ArgumentParser();
            parser.AddArgument("username", "The username for the router login", false, "admin");
            parser.AddArgument("password", "The password for the router login", false, "admin");
            parser.AddArgument("ip", "The custom router IP address", false, "");
            parser.AddArgument("router", "The router model (ch7465vf, f660, tl-mr105)", false, "ch7465vf");

            var result = parser.ParseArgs(args);

            Assert.That(result.Get<string>("username"), Is.EqualTo("admin"));
            Assert.That(result.Get<string>("password"), Is.EqualTo("admin"));
            Assert.That(result.Get<string>("ip"), Is.EqualTo(""));
            Assert.That(result.Get<string>("router"), Is.EqualTo("ch7465vf"));
        }

        [Test]
        public void ParseArguments_WithCustomUsername_ReturnsCustomValue()
        {
            var args = new[] { "--username", "customuser" };
            var parser = new ArgumentParser();
            parser.AddArgument("username", "The username for the router login", false, "admin");
            parser.AddArgument("password", "The password for the router login", false, "admin");
            parser.AddArgument("ip", "The custom router IP address", false, "");
            parser.AddArgument("router", "The router model (ch7465vf, f660, tl-mr105)", false, "ch7465vf");

            var result = parser.ParseArgs(args);

            Assert.That(result.Get<string>("username"), Is.EqualTo("customuser"));
        }

        [Test]
        public void ParseArguments_WithCustomPassword_ReturnsCustomValue()
        {
            var args = new[] { "--password", "custompass" };
            var parser = new ArgumentParser();
            parser.AddArgument("username", "The username for the router login", false, "admin");
            parser.AddArgument("password", "The password for the router login", false, "admin");
            parser.AddArgument("ip", "The custom router IP address", false, "");
            parser.AddArgument("router", "The router model (ch7465vf, f660, tl-mr105)", false, "ch7465vf");

            var result = parser.ParseArgs(args);

            Assert.That(result.Get<string>("password"), Is.EqualTo("custompass"));
        }

        [Test]
        public void ParseArguments_WithCustomIp_ReturnsCustomValue()
        {
            var args = new[] { "--ip", "10.0.0.1" };
            var parser = new ArgumentParser();
            parser.AddArgument("username", "The username for the router login", false, "admin");
            parser.AddArgument("password", "The password for the router login", false, "admin");
            parser.AddArgument("ip", "The custom router IP address", false, "");
            parser.AddArgument("router", "The router model (ch7465vf, f660, tl-mr105)", false, "ch7465vf");

            var result = parser.ParseArgs(args);

            Assert.That(result.Get<string>("ip"), Is.EqualTo("10.0.0.1"));
        }

        [Test]
        public void ParseArguments_WithCustomRouter_ReturnsCustomValue()
        {
            var args = new[] { "--router", "f660" };
            var parser = new ArgumentParser();
            parser.AddArgument("username", "The username for the router login", false, "admin");
            parser.AddArgument("password", "The password for the router login", false, "admin");
            parser.AddArgument("ip", "The custom router IP address", false, "");
            parser.AddArgument("router", "The router model (ch7465vf, f660, tl-mr105)", false, "ch7465vf");

            var result = parser.ParseArgs(args);

            Assert.That(result.Get<string>("router"), Is.EqualTo("f660"));
        }

        [Test]
        public void ParseArguments_WithAllCustomValues_ReturnsAllCustomValues()
        {
            var args = new[] { "--username", "user", "--password", "pass", "--ip", "192.168.1.1", "--router", "tl-mr105" };
            var parser = new ArgumentParser();
            parser.AddArgument("username", "The username for the router login", false, "admin");
            parser.AddArgument("password", "The password for the router login", false, "admin");
            parser.AddArgument("ip", "The custom router IP address", false, "");
            parser.AddArgument("router", "The router model (ch7465vf, f660, tl-mr105)", false, "ch7465vf");

            var result = parser.ParseArgs(args);

            Assert.That(result.Get<string>("username"), Is.EqualTo("user"));
            Assert.That(result.Get<string>("password"), Is.EqualTo("pass"));
            Assert.That(result.Get<string>("ip"), Is.EqualTo("192.168.1.1"));
            Assert.That(result.Get<string>("router"), Is.EqualTo("tl-mr105"));
        }

        [Test]
        public void ParseArguments_CaseInsensitiveRouterValue()
        {
            var args = new[] { "--router", "CH7465VF" };
            var parser = new ArgumentParser();
            parser.AddArgument("username", "The username for the router login", false, "admin");
            parser.AddArgument("password", "The password for the router login", false, "admin");
            parser.AddArgument("ip", "The custom router IP address", false, "");
            parser.AddArgument("router", "The router model (ch7465vf, f660, tl-mr105)", false, "ch7465vf");

            var result = parser.ParseArgs(args);

            Assert.That(result.Get<string>("router"), Is.EqualTo("CH7465VF"));
        }
    }

    public sealed class ProgramDeviceParsingTests
    {
        [Test]
        public void ParseDeviceArgument_ch7465vf_ReturnsLowercase()
        {
            var args = new ArgumentsCollection();
            args["router"] = "ch7465vf";
            var result = ParseDeviceArgument(args);
            Assert.That(result, Is.EqualTo("ch7465vf"));
        }

        [Test]
        public void ParseDeviceArgument_CH7465VF_ReturnsLowercase()
        {
            var args = new ArgumentsCollection();
            args["router"] = "CH7465VF";
            var result = ParseDeviceArgument(args);
            Assert.That(result, Is.EqualTo("ch7465vf"));
        }

        [Test]
        public void ParseDeviceArgument_f660_ReturnsLowercase()
        {
            var args = new ArgumentsCollection();
            args["router"] = "f660";
            var result = ParseDeviceArgument(args);
            Assert.That(result, Is.EqualTo("f660"));
        }

        [Test]
        public void ParseDeviceArgument_F660_ReturnsLowercase()
        {
            var args = new ArgumentsCollection();
            args["router"] = "F660";
            var result = ParseDeviceArgument(args);
            Assert.That(result, Is.EqualTo("f660"));
        }

        [Test]
        public void ParseDeviceArgument_tl_mr105_ReturnsLowercase()
        {
            var args = new ArgumentsCollection();
            args["router"] = "tl-mr105";
            var result = ParseDeviceArgument(args);
            Assert.That(result, Is.EqualTo("tl-mr105"));
        }

        [Test]
        public void ParseDeviceArgument_TL_MR105_ReturnsLowercase()
        {
            var args = new ArgumentsCollection();
            args["router"] = "TL-MR105";
            var result = ParseDeviceArgument(args);
            Assert.That(result, Is.EqualTo("tl-mr105"));
        }

        [Test]
        public void ParseDeviceArgument_InvalidValue_ThrowsArgumentException()
        {
            var args = new ArgumentsCollection();
            args["router"] = "invalid";
            var ex = Assert.Throws<ArgumentException>(() => ParseDeviceArgument(args));
            Assert.That(ex.Message, Does.Contain("Unknown device 'invalid'"));
            Assert.That(ex.Message, Does.Contain("ch7465vf"));
            Assert.That(ex.Message, Does.Contain("f660"));
            Assert.That(ex.Message, Does.Contain("tl-mr105"));
        }

        [Test]
        public void ParseDeviceArgument_EmptyString_ThrowsArgumentException()
        {
            var args = new ArgumentsCollection();
            args["router"] = "";
            var ex = Assert.Throws<ArgumentException>(() => ParseDeviceArgument(args));
            Assert.That(ex.Message, Does.Contain("Unknown device ''"));
        }

        private static string ParseDeviceArgument(ArgumentsCollection arguments)
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
    }

    public sealed class ProgramRetrieveAccessInfoTests
    {
        [Test]
        public void RetrieveAccessInfo_ReturnsCorrectValues()
        {
            var args = new ArgumentsCollection();
            args["username"] = "testuser";
            args["password"] = "testpass";
            args["ip"] = "192.168.1.100";

            var result = RetrieveAccessInfo(args);

            Assert.That(result.Username, Is.EqualTo("testuser"));
            Assert.That(result.Password, Is.EqualTo("testpass"));
            Assert.That(result.IpAddress, Is.EqualTo("192.168.1.100"));
        }

        [Test]
        public void RetrieveAccessInfo_WithEmptyIp_ReturnsEmptyString()
        {
            var args = new ArgumentsCollection();
            args["username"] = "testuser";
            args["password"] = "testpass";
            args["ip"] = "";

            var result = RetrieveAccessInfo(args);

            Assert.That(result.IpAddress, Is.EqualTo(""));
        }

        [Test]
        public void RetrieveAccessInfo_WithNullIp_ReturnsNull()
        {
            var args = new ArgumentsCollection();
            args["username"] = "testuser";
            args["password"] = "testpass";
            args["ip"] = null;

            var result = RetrieveAccessInfo(args);

            Assert.That(result.IpAddress, Is.Null);
        }

        private static RouterAccessInfo RetrieveAccessInfo(ArgumentsCollection arguments)
        {
            return new RouterAccessInfo
            {
                Username = arguments.Get<string>("username"),
                Password = arguments.Get<string>("password"),
                IpAddress = arguments.Get<string>("ip")
            };
        }
    }
}
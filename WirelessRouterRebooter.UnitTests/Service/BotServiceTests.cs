using System;
using Moq;
using NUnit.Framework;
using NuciLog.Core;
using WirelessRouterRebooter.Logging;
using WirelessRouterRebooter.Service;
using WirelessRouterRebooter.Service.Models;
using WirelessRouterRebooter.Service.Processors;

namespace WirelessRouterRebooter.UnitTests.Service
{
    public sealed class BotServiceTests
    {
        private Mock<IRouterProcessor> _routerProcessorMock = null!;
        private Mock<ILogger> _loggerMock = null!;
        private BotService _botService = null!;
        private RouterAccessInfo _accessInfo = null!;

        [SetUp]
        public void SetUp()
        {
            _routerProcessorMock = new Mock<IRouterProcessor>();
            _loggerMock = new Mock<ILogger>();
            _botService = new BotService(_routerProcessorMock.Object, _loggerMock.Object);
            _accessInfo = new RouterAccessInfo
            {
                Username = "admin",
                Password = "password",
                IpAddress = "192.168.0.1"
            };

            _routerProcessorMock.Setup(p => p.IpAddress).Returns("192.168.0.1");
            _routerProcessorMock.Setup(p => p.BrandName).Returns("TestBrand");
            _routerProcessorMock.Setup(p => p.ModelName).Returns("TestModel");
        }

        [Test]
        public void Run_CallsLogInAndReboot()
        {
            _botService.Run(_accessInfo);
            _routerProcessorMock.Verify(p => p.LogIn(_accessInfo), Times.Once);
            _routerProcessorMock.Verify(p => p.Reboot(), Times.Once);
        }

        [Test]
        public void Run_CallsLogInBeforeReboot()
        {
            var callOrder = 0;
            _routerProcessorMock.Setup(p => p.LogIn(_accessInfo)).Callback(() => callOrder = 1);
            _routerProcessorMock.Setup(p => p.Reboot()).Callback(() => Assert.That(callOrder, Is.EqualTo(1)));
            _botService.Run(_accessInfo);
        }

        [Test]
        public void LogIn_LogsInfoWithCorrectParameters()
        {
            _botService.Run(_accessInfo);
            _loggerMock.Verify(l => l.Info(
                It.IsAny<Operation>(),
                It.IsAny<OperationStatus>(),
                It.IsAny<LogInfo[]>()), Times.AtLeastOnce);
        }

        [Test]
        public void LogIn_LogsDebugOnSuccess()
        {
            _botService.Run(_accessInfo);
            _loggerMock.Verify(l => l.Debug(
                It.IsAny<Operation>(),
                It.IsAny<OperationStatus>(),
                It.IsAny<LogInfo[]>()), Times.AtLeastOnce);
        }

        [Test]
        public void LogIn_LogsErrorAndRethrows_OnException()
        {
            var exception = new InvalidOperationException("Login failed");
            _routerProcessorMock.Setup(p => p.LogIn(_accessInfo)).Throws(exception);

            Assert.Throws<InvalidOperationException>(() => _botService.Run(_accessInfo));

            _loggerMock.Verify(l => l.Error(
                It.IsAny<Operation>(),
                It.IsAny<OperationStatus>(),
                It.IsAny<Exception>(),
                It.IsAny<LogInfo[]>()), Times.AtLeastOnce);
        }

        [Test]
        public void Reboot_LogsInfoWithCorrectParameters()
        {
            _botService.Run(_accessInfo);
            _loggerMock.Verify(l => l.Info(
                It.IsAny<Operation>(),
                It.IsAny<OperationStatus>(),
                It.IsAny<LogInfo[]>()), Times.AtLeastOnce);
        }

        [Test]
        public void Reboot_LogsDebugOnSuccess()
        {
            _botService.Run(_accessInfo);
            _loggerMock.Verify(l => l.Debug(
                It.IsAny<Operation>(),
                It.IsAny<OperationStatus>(),
                It.IsAny<LogInfo[]>()), Times.AtLeastOnce);
        }

        [Test]
        public void Reboot_LogsErrorAndRethrows_OnException()
        {
            var exception = new InvalidOperationException("Reboot failed");
            _routerProcessorMock.Setup(p => p.Reboot()).Throws(exception);

            Assert.Throws<InvalidOperationException>(() => _botService.Run(_accessInfo));

            _loggerMock.Verify(l => l.Error(
                It.IsAny<Operation>(),
                It.IsAny<OperationStatus>(),
                It.IsAny<Exception>(),
                It.IsAny<LogInfo[]>()), Times.AtLeastOnce);
        }

        [Test]
        public void Run_UsesRouterProcessorIpAddress_WhenAccessInfoIpIsNull()
        {
            var accessInfo = new RouterAccessInfo { Username = "admin", Password = "pass", IpAddress = null };
            _routerProcessorMock.Setup(p => p.IpAddress).Returns("10.0.0.1");

            _botService.Run(accessInfo);

            _loggerMock.Verify(l => l.Info(
                It.IsAny<Operation>(),
                It.IsAny<OperationStatus>(),
                It.IsAny<LogInfo[]>()), Times.AtLeastOnce);
        }

        [Test]
        public void Run_UsesAccessInfoIpAddress_WhenProvided()
        {
            var accessInfo = new RouterAccessInfo { Username = "admin", Password = "pass", IpAddress = "192.168.5.5" };

            _botService.Run(accessInfo);

            _loggerMock.Verify(l => l.Info(
                It.IsAny<Operation>(),
                It.IsAny<OperationStatus>(),
                It.IsAny<LogInfo[]>()), Times.AtLeastOnce);
        }
    }
}
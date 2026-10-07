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
        private readonly Mock<IRouterProcessor> _routerProcessorMock;
        private readonly Mock<ILogger> _loggerMock;
        private readonly BotService _botService;
        private readonly RouterAccessInfo _accessInfo;

        public BotServiceTests()
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
                MyOperation.LogIn,
                OperationStatus.Started,
                It.Is<LogInfo>(li => li.Key == MyLogInfoKey.IpAddress && li.Value == "192.168.0.1"),
                It.Is<LogInfo>(li => li.Key == MyLogInfoKey.BrandName && li.Value == "TestBrand"),
                It.Is<LogInfo>(li => li.Key == MyLogInfoKey.ModelName && li.Value == "TestModel"),
                It.Is<LogInfo>(li => li.Key == MyLogInfoKey.Username && li.Value == "admin")), Times.Once);
        }

        [Test]
        public void LogIn_LogsDebugOnSuccess()
        {
            _botService.Run(_accessInfo);
            _loggerMock.Verify(l => l.Debug(
                MyOperation.LogIn,
                OperationStatus.Success,
                It.IsAny<LogInfo[]>()), Times.Once);
        }

        [Test]
        public void LogIn_LogsErrorAndRethrows_OnException()
        {
            var exception = new InvalidOperationException("Login failed");
            _routerProcessorMock.Setup(p => p.LogIn(_accessInfo)).Throws(exception);

            Assert.Throws<InvalidOperationException>(() => _botService.Run(_accessInfo));

            _loggerMock.Verify(l => l.Error(
                MyOperation.LogIn,
                OperationStatus.Failure,
                exception,
                It.IsAny<LogInfo[]>()), Times.Once);
        }

        [Test]
        public void Reboot_LogsInfoWithCorrectParameters()
        {
            _botService.Run(_accessInfo);
            _loggerMock.Verify(l => l.Info(
                MyOperation.Reboot,
                OperationStatus.Started,
                It.Is<LogInfo>(li => li.Key == MyLogInfoKey.IpAddress && li.Value == "192.168.0.1"),
                It.Is<LogInfo>(li => li.Key == MyLogInfoKey.BrandName && li.Value == "TestBrand"),
                It.Is<LogInfo>(li => li.Key == MyLogInfoKey.ModelName && li.Value == "TestModel")), Times.Once);
        }

        [Test]
        public void Reboot_LogsDebugOnSuccess()
        {
            _botService.Run(_accessInfo);
            _loggerMock.Verify(l => l.Debug(
                MyOperation.Reboot,
                OperationStatus.Success,
                It.IsAny<LogInfo[]>()), Times.Once);
        }

        [Test]
        public void Reboot_LogsErrorAndRethrows_OnException()
        {
            var exception = new InvalidOperationException("Reboot failed");
            _routerProcessorMock.Setup(p => p.Reboot()).Throws(exception);

            Assert.Throws<InvalidOperationException>(() => _botService.Run(_accessInfo));

            _loggerMock.Verify(l => l.Error(
                MyOperation.Reboot,
                OperationStatus.Failure,
                exception,
                It.IsAny<LogInfo[]>()), Times.Once);
        }

        [Test]
        public void Run_UsesRouterProcessorIpAddress_WhenAccessInfoIpIsNull()
        {
            var accessInfo = new RouterAccessInfo { Username = "admin", Password = "pass", IpAddress = null };
            _routerProcessorMock.Setup(p => p.IpAddress).Returns("10.0.0.1");

            _botService.Run(accessInfo);

            _loggerMock.Verify(l => l.Info(
                MyOperation.LogIn,
                OperationStatus.Started,
                It.Is<LogInfo>(li => li.Key == MyLogInfoKey.IpAddress && li.Value == "10.0.0.1"),
                It.IsAny<LogInfo[]>()));
        }

        [Test]
        public void Run_UsesAccessInfoIpAddress_WhenProvided()
        {
            var accessInfo = new RouterAccessInfo { Username = "admin", Password = "pass", IpAddress = "192.168.5.5" };

            _botService.Run(accessInfo);

            _loggerMock.Verify(l => l.Info(
                MyOperation.LogIn,
                OperationStatus.Started,
                It.Is<LogInfo>(li => li.Key == MyLogInfoKey.IpAddress && li.Value == "192.168.5.5"),
                It.IsAny<LogInfo[]>()));
        }
    }
}
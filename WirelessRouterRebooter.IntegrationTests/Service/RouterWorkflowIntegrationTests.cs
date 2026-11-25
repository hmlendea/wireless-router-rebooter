using System;

using Microsoft.Extensions.DependencyInjection;

using Moq;

using NUnit.Framework;

using NuciLog.Core;
using NuciWeb;
using NuciWeb.Automation;

using WirelessRouterRebooter.Logging;
using WirelessRouterRebooter.Service;
using WirelessRouterRebooter.Service.Models;
using WirelessRouterRebooter.Service.Processors;

namespace WirelessRouterRebooter.IntegrationTests.Service
{
    [TestFixture]
    public sealed class RouterWorkflowIntegrationTests
    {
        private Mock<IWebProcessor> webProcessorMock = null!;
        private Mock<ILogger> loggerMock = null!;
        private RouterAccessInfo accessInfo = null!;

        [SetUp]
        public void SetUp()
        {
            webProcessorMock = new Mock<IWebProcessor>();
            loggerMock = new Mock<ILogger>();
            accessInfo = new RouterAccessInfo
            {
                Username = "DummyUser",
                Password = "dummy-cert.pem",
                IpAddress = null
            };
        }

        [TestCase("ch7465vf", "Compal", "CH7465VF", "192.168.0.1")]
        [TestCase("f660", "ZTE", "F660", "192.168.1.1")]
        [TestCase("tl-mr105", "TP-Link", "MR105", "192.168.0.1")]
        public void GivenARegisteredRouter_WhenRunningTheWorkflow_ThenItLogsInAndReboots(
            string routerKey,
            string expectedBrandName,
            string expectedModelName,
            string expectedIpAddress)
        {
            IRouterProcessor processor = BuildProcessor(routerKey, accessInfo);
            BotService service = new(processor, loggerMock.Object);
            int completedSteps = 0;

            webProcessorMock.Setup(w => w.Click(It.IsAny<string>()))
                .Callback(() => completedSteps += 1);

            service.Run(accessInfo);

            Assert.That(processor.BrandName, Is.EqualTo(expectedBrandName));
            Assert.That(processor.ModelName, Is.EqualTo(expectedModelName));
            Assert.That(processor.IpAddress, Is.EqualTo(expectedIpAddress));
            Assert.That(completedSteps, Is.GreaterThan(0));
            loggerMock.Verify(l => l.Debug(
                It.IsAny<Operation>(),
                It.IsAny<OperationStatus>(),
                It.IsAny<LogInfo[]>()), Times.Exactly(2));
        }

        [TestCase("ch7465vf", "10.0.0.1")]
        [TestCase("f660", "10.0.0.1")]
        [TestCase("tl-mr105", "10.0.0.1")]
        public void GivenACustomIpAddress_WhenBuildingAProcessor_ThenTheWorkflowUsesIt(
            string routerKey,
            string customIpAddress)
        {
            accessInfo.IpAddress = customIpAddress;
            IRouterProcessor processor = BuildProcessor(routerKey, accessInfo);
            BotService service = new(processor, loggerMock.Object);

            service.Run(accessInfo);

            Assert.That(processor.IpAddress, Is.EqualTo(customIpAddress));
            webProcessorMock.Verify(w => w.GoToUrl($"http://{customIpAddress}/"), Times.Once);
        }

        [Test]
        public void GivenAConfirmDialog_WhenRunningTheTpLinkLogin_ThenItConfirmsAndWaitsForThePanel()
        {
            IRouterProcessor processor = BuildProcessor("tl-mr105", accessInfo);
            webProcessorMock.Setup(w => w.IsElementVisible(It.IsAny<string>())).Returns(true);

            processor.LogIn(accessInfo);

            webProcessorMock.Verify(w => w.WaitForAllElementsToBeVisible(Select.ById("lte_panel")), Times.Once);
        }

        [Test]
        public void GivenNoConfirmDialog_WhenRunningTheTpLinkLogin_ThenItDoesNotWaitForThePanel()
        {
            IRouterProcessor processor = BuildProcessor("tl-mr105", accessInfo);
            webProcessorMock.Setup(w => w.IsElementVisible(It.IsAny<string>())).Returns(false);

            processor.LogIn(accessInfo);

            webProcessorMock.Verify(w => w.WaitForAllElementsToBeVisible(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void GivenALoginFailure_WhenRunningTheWorkflow_ThenRebootIsNotAttempted()
        {
            IRouterProcessor processor = BuildProcessor("f660", accessInfo);
            InvalidOperationException exception = new("Login failed");
            webProcessorMock.Setup(w => w.SetText(It.IsAny<string>(), It.IsAny<string>())).Throws(exception);
            BotService service = new(processor, loggerMock.Object);

            InvalidOperationException actual = Assert.Throws<InvalidOperationException>(() => service.Run(accessInfo))!;

            Assert.That(actual, Is.SameAs(exception));
            webProcessorMock.Verify(w => w.AcceptAlert(), Times.Never);
            loggerMock.Verify(l => l.Error(
                It.IsAny<Operation>(),
                It.IsAny<OperationStatus>(),
                exception,
                It.IsAny<LogInfo[]>()), Times.Once);
        }

        [Test]
        public void GivenARebootFailure_WhenRunningTheWorkflow_ThenTheFailureIsPropagated()
        {
            IRouterProcessor processor = BuildProcessor("f660", accessInfo);
            InvalidOperationException exception = new("Reboot failed");
            webProcessorMock.Setup(w => w.Click(Select.ById("Submit1"))).Throws(exception);
            BotService service = new(processor, loggerMock.Object);

            InvalidOperationException actual = Assert.Throws<InvalidOperationException>(() => service.Run(accessInfo))!;

            Assert.That(actual, Is.SameAs(exception));
            loggerMock.Verify(l => l.Error(
                It.IsAny<Operation>(),
                It.IsAny<OperationStatus>(),
                exception,
                It.IsAny<LogInfo[]>()), Times.Once);
        }

        private IRouterProcessor BuildProcessor(string routerKey, RouterAccessInfo routerAccessInfo)
        {
            ServiceCollection services = new();
            services.AddSingleton(webProcessorMock.Object);
            services.AddSingleton(routerAccessInfo);
            services.AddKeyedTransient<IRouterProcessor, CompalCH7465VF>("ch7465vf");
            services.AddKeyedTransient<IRouterProcessor, TpLinkMR105Processor>("tl-mr105");
            services.AddKeyedTransient<IRouterProcessor, ZteF660>("f660");

            ServiceProvider provider = services.BuildServiceProvider();
            return provider.GetRequiredKeyedService<IRouterProcessor>(routerKey);
        }
    }
}

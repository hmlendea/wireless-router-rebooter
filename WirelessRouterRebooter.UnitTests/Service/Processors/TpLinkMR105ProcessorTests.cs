using System;
using Moq;
using NUnit.Framework;
using NuciWeb;
using NuciWeb.Automation;
using WirelessRouterRebooter.Service.Models;
using WirelessRouterRebooter.Service.Processors;

namespace WirelessRouterRebooter.UnitTests.Service.Processors
{
    public sealed class TpLinkMR105ProcessorTests
    {
        private readonly Mock<IWebProcessor> _webProcessorMock;
        private readonly RouterAccessInfo _accessInfo;
        private readonly TpLinkMR105Processor _processor;

        public TpLinkMR105ProcessorTests()
        {
            _webProcessorMock = new Mock<IWebProcessor>();
            _accessInfo = new RouterAccessInfo
            {
                Username = "admin",
                Password = "password",
                IpAddress = null
            };
            _processor = new TpLinkMR105Processor(_webProcessorMock.Object, _accessInfo);
        }

        [Test]
        public void Constructor_SetsBrandNameCorrectly()
        {
            Assert.That(_processor.BrandName, Is.EqualTo("TP-Link"));
        }

        [Test]
        public void Constructor_SetsModelNameCorrectly()
        {
            Assert.That(_processor.ModelName, Is.EqualTo("MR105"));
        }

        [Test]
        public void Constructor_SetsDefaultIpAddress_WhenAccessInfoIpIsNull()
        {
            Assert.That(_processor.IpAddress, Is.EqualTo("192.168.0.1"));
        }

        [Test]
        public void Constructor_UsesAccessInfoIpAddress_WhenProvided()
        {
            var accessInfo = new RouterAccessInfo { IpAddress = "10.0.0.1" };
            var processor = new TpLinkMR105Processor(_webProcessorMock.Object, accessInfo);
            Assert.That(processor.IpAddress, Is.EqualTo("10.0.0.1"));
        }

        [Test]
        public void LogIn_CallsGoToUrlWithCorrectUrl()
        {
            _processor.LogIn(_accessInfo);
            _webProcessorMock.Verify(w => w.GoToUrl("http://192.168.0.1/"), Times.Once);
        }

        [Test]
        public void LogIn_SetsPasswordField()
        {
            _processor.LogIn(_accessInfo);
            _webProcessorMock.Verify(w => w.SetText(Select.ById("pc-login-password"), "password"), Times.Once);
        }

        [Test]
        public void LogIn_ClicksLoginButton()
        {
            _processor.LogIn(_accessInfo);
            _webProcessorMock.Verify(w => w.Click(Select.ById("pc-login-btn")), Times.Once);
        }

        [Test]
        public void LogIn_WaitsForEitherConfirmButtonOrLtePanel()
        {
            _processor.LogIn(_accessInfo);
            _webProcessorMock.Verify(w => w.WaitForAnyElementToBeVisible(
                Select.ByXPath("//*[@id='alert-container']/div/div[@class='position-center-left']/div/div[@class='msg-btn-container']/div/div[2]/button"),
                Select.ById("lte_panel")), Times.Once);
        }

        [Test]
        public void LogIn_ClicksConfirmButton_WhenVisible()
        {
            _webProcessorMock.Setup(w => w.IsElementVisible(It.IsAny<string>())).Returns(true);
            _processor.LogIn(_accessInfo);
            _webProcessorMock.Verify(w => w.Click(Select.ByXPath("//*[@id='alert-container']/div/div[@class='position-center-left']/div/div[@class='msg-btn-container']/div/div[2]/button")), Times.Once);
            _webProcessorMock.Verify(w => w.WaitForAllElementsToBeVisible(Select.ById("lte_panel")), Times.Once);
        }

        [Test]
        public void LogIn_SkipsConfirmButton_WhenNotVisible()
        {
            _webProcessorMock.Setup(w => w.IsElementVisible(It.IsAny<string>())).Returns(false);
            _processor.LogIn(_accessInfo);
            _webProcessorMock.Verify(w => w.Click(It.Is<string>(s => s.Contains("alert-container"))), Times.Never);
            _webProcessorMock.Verify(w => w.WaitForAllElementsToBeVisible(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void LogIn_UsesAccessInfoPassword()
        {
            var accessInfo = new RouterAccessInfo { Password = "custompass" };
            _processor.LogIn(accessInfo);
            _webProcessorMock.Verify(w => w.SetText(Select.ById("pc-login-password"), "custompass"), Times.Once);
        }

        [Test]
        public void Reboot_ClicksRebootIcon()
        {
            _processor.Reboot();
            _webProcessorMock.Verify(w => w.Click(Select.ByXPath("//*[@id='topReboot']/span[@class='icon']")), Times.Once);
        }

        [Test]
        public void Reboot_ClicksConfirmButton()
        {
            _processor.Reboot();
            _webProcessorMock.Verify(w => w.Click(Select.ByXPath("//*[@id='alert-container']/div/div[@class='position-center-left']/div/div[@class='msg-btn-container']/div/div[2]/button")), Times.Once);
        }

        [Test]
        public void Reboot_Waits5Seconds()
        {
            _processor.Reboot();
            _webProcessorMock.Verify(w => w.Wait(TimeSpan.FromSeconds(5)), Times.Once);
        }

        [Test]
        public void Reboot_TotalClickCalls_2()
        {
            _processor.Reboot();
            _webProcessorMock.Verify(w => w.Click(It.IsAny<string>()), Times.Exactly(2));
        }
    }
}
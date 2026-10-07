using System;

using Moq;

using NUnit.Framework;

using NuciWeb;
using NuciWeb.Automation;

using WirelessRouterRebooter.Service.Models;
using WirelessRouterRebooter.Service.Processors;

namespace WirelessRouterRebooter.UnitTests.Service.Processors
{
    public sealed class ZteF660Tests
    {
        private readonly Mock<IWebProcessor> webProcessorMock;
        private readonly RouterAccessInfo accessInfo;
        private readonly ZteF660 processor;

        public ZteF660Tests()
        {
            webProcessorMock = new Mock<IWebProcessor>();
            accessInfo = new RouterAccessInfo
            {
                Username = "admin",
                Password = "password",
                IpAddress = null
            };
            processor = new ZteF660(webProcessorMock.Object, accessInfo);
        }

        [Test]
        public void Constructor_SetsBrandNameCorrectly()
        {
            Assert.That(processor.BrandName, Is.EqualTo("ZTE"));
        }

        [Test]
        public void Constructor_SetsModelNameCorrectly()
        {
            Assert.That(processor.ModelName, Is.EqualTo("F660"));
        }

        [Test]
        public void Constructor_SetsDefaultIpAddress_WhenAccessInfoIpIsNull()
        {
            Assert.That(processor.IpAddress, Is.EqualTo("192.168.1.1"));
        }

        [Test]
        public void Constructor_UsesAccessInfoIpAddress_WhenProvided()
        {
            var accessInfo = new RouterAccessInfo { IpAddress = "10.0.0.1" };
            var processor = new ZteF660(webProcessorMock.Object, accessInfo);
            Assert.That(processor.IpAddress, Is.EqualTo("10.0.0.1"));
        }

        [Test]
        public void LogIn_CallsGoToUrlWithCorrectUrl()
        {
            processor.LogIn(accessInfo);
            webProcessorMock.Verify(w => w.GoToUrl("http://192.168.1.1/"), Times.Once);
        }

        [Test]
        public void LogIn_SetsUsernameField()
        {
            processor.LogIn(accessInfo);
            webProcessorMock.Verify(w => w.SetText(Select.ById("Frm_Username"), "admin"), Times.Once);
        }

        [Test]
        public void LogIn_SetsPasswordField()
        {
            processor.LogIn(accessInfo);
            webProcessorMock.Verify(w => w.SetText(Select.ById("Frm_Password"), "password"), Times.Once);
        }

        [Test]
        public void LogIn_ClicksLoginButton()
        {
            processor.LogIn(accessInfo);
            webProcessorMock.Verify(w => w.Click(Select.ById("LoginId")), Times.Once);
        }

        [Test]
        public void LogIn_WaitsForLoginElementToDisappear()
        {
            processor.LogIn(accessInfo);
            webProcessorMock.Verify(w => w.WaitForElementToDisappear(Select.ById("LoginId")), Times.Once);
        }

        [Test]
        public void LogIn_UsesAccessInfoCredentials()
        {
            var accessInfo = new RouterAccessInfo { Username = "user", Password = "pass" };
            processor.LogIn(accessInfo);
            webProcessorMock.Verify(w => w.SetText(Select.ById("Frm_Username"), "user"), Times.Once);
            webProcessorMock.Verify(w => w.SetText(Select.ById("Frm_Password"), "pass"), Times.Once);
        }

        [Test]
        public void Reboot_CallsGoToUrlWithRebootPage()
        {
            processor.Reboot();
            webProcessorMock.Verify(w => w.GoToUrl("http://192.168.1.1/getpage.gch?pid=1002&nextpage=manager_dev_conf_t.gch"), Times.Once);
        }

        [Test]
        public void Reboot_ClicksSubmitButton()
        {
            processor.Reboot();
            webProcessorMock.Verify(w => w.Click(Select.ById("Submit1")), Times.Once);
        }

        [Test]
        public void Reboot_AcceptsAlert()
        {
            processor.Reboot();
            webProcessorMock.Verify(w => w.AcceptAlert(), Times.Once);
        }

        [Test]
        public void Reboot_Waits3Seconds()
        {
            processor.Reboot();
            webProcessorMock.Verify(w => w.Wait(TimeSpan.FromSeconds(3)), Times.Once);
        }

        [Test]
        public void Reboot_TotalGoToUrlCalls_1()
        {
            processor.Reboot();
            webProcessorMock.Verify(w => w.GoToUrl(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void Reboot_TotalClickCalls_1()
        {
            processor.Reboot();
            webProcessorMock.Verify(w => w.Click(It.IsAny<string>()), Times.Once);
        }
    }
}
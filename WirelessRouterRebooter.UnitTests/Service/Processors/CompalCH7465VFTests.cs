using Moq;

using NuciWeb;
using NuciWeb.Automation;

using NUnit.Framework;

using WirelessRouterRebooter.Service.Models;
using WirelessRouterRebooter.Service.Processors;

namespace WirelessRouterRebooter.UnitTests.Service.Processors
{
    public sealed class CompalCH7465VFTests
    {
        private readonly Mock<IWebProcessor> webProcessorMock;
        private readonly RouterAccessInfo accessInfo;
        private readonly CompalCH7465VF processor;

        public CompalCH7465VFTests()
        {
            webProcessorMock = new Mock<IWebProcessor>();
            accessInfo = new RouterAccessInfo
            {
                Username = "admin",
                Password = "password",
                IpAddress = null
            };
            processor = new CompalCH7465VF(webProcessorMock.Object, accessInfo);
        }

        [Test]
        public void Constructor_SetsBrandNameCorrectly()
        {
            Assert.That(processor.BrandName, Is.EqualTo("Compal"));
        }

        [Test]
        public void Constructor_SetsModelNameCorrectly()
        {
            Assert.That(processor.ModelName, Is.EqualTo("CH7465VF"));
        }

        [Test]
        public void Constructor_SetsDefaultIpAddress_WhenAccessInfoIpIsNull()
        {
            Assert.That(processor.IpAddress, Is.EqualTo("192.168.0.1"));
        }

        [Test]
        public void Constructor_UsesAccessInfoIpAddress_WhenProvided()
        {
            var accessInfo = new RouterAccessInfo { IpAddress = "10.0.0.1" };
            var processor = new CompalCH7465VF(webProcessorMock.Object, accessInfo);
            Assert.That(processor.IpAddress, Is.EqualTo("10.0.0.1"));
        }

        [Test]
        public void LogIn_CallsGoToUrlWithCorrectUrl()
        {
            processor.LogIn(accessInfo);
            webProcessorMock.Verify(w => w.GoToUrl("http://192.168.0.1/"), Times.Once);
        }

        [Test]
        public void LogIn_CallsWaitWith5000ms()
        {
            processor.LogIn(accessInfo);
            webProcessorMock.Verify(w => w.Wait(5000), Times.Once);
        }

        [Test]
        public void LogIn_SetsUsernameField()
        {
            processor.LogIn(accessInfo);
            webProcessorMock.Verify(w => w.SetText(Select.ByName("loginUsername"), "admin"), Times.Once);
        }

        [Test]
        public void LogIn_SetsPasswordField()
        {
            processor.LogIn(accessInfo);
            webProcessorMock.Verify(w => w.SetText(Select.ByName("loginPassword"), "password"), Times.Once);
        }

        [Test]
        public void LogIn_ClicksLoginButton()
        {
            processor.LogIn(accessInfo);
            webProcessorMock.Verify(w => w.Click(Select.ById("c_42")), Times.Once);
        }

        [Test]
        public void LogIn_UsesAccessInfoCredentials()
        {
            var accessInfo = new RouterAccessInfo { Username = "user", Password = "pass" };
            processor.LogIn(accessInfo);
            webProcessorMock.Verify(w => w.SetText(Select.ByName("loginUsername"), "user"), Times.Once);
            webProcessorMock.Verify(w => w.SetText(Select.ByName("loginPassword"), "pass"), Times.Once);
        }

        [Test]
        public void Reboot_CallsWaitWith5000msInitially()
        {
            processor.Reboot();
            webProcessorMock.Verify(w => w.Wait(5000), Times.Once);
        }

        [Test]
        public void Reboot_ClicksMenuItem1ThreeTimes()
        {
            processor.Reboot();
            webProcessorMock.Verify(w => w.Click(Select.ById("c_mu25")), Times.Exactly(3));
        }

        [Test]
        public void Reboot_Waits250msBetweenMenuItem1Clicks()
        {
            processor.Reboot();
            webProcessorMock.Verify(w => w.Wait(250), Times.Exactly(3));
        }

        [Test]
        public void Reboot_Waits1000msAfterFirstLoop()
        {
            processor.Reboot();
            webProcessorMock.Verify(w => w.Wait(1000), Times.Once);
        }

        [Test]
        public void Reboot_ClicksMenuItem2ThreeTimes()
        {
            processor.Reboot();
            webProcessorMock.Verify(w => w.Click(Select.ById("c_mu27")), Times.Exactly(3));
        }

        [Test]
        public void Reboot_Waits250msBetweenMenuItem2Clicks()
        {
            processor.Reboot();
            webProcessorMock.Verify(w => w.Wait(250), Times.Exactly(3));
        }

        [Test]
        public void Reboot_ClicksRebootButton()
        {
            processor.Reboot();
            webProcessorMock.Verify(w => w.Click(Select.ById("c_rr14")), Times.Once);
        }

        [Test]
        public void Reboot_TotalWaitCalls_8()
        {
            processor.Reboot();
            webProcessorMock.Verify(w => w.Wait(It.IsAny<int>()), Times.Exactly(8));
        }

        [Test]
        public void Reboot_TotalClickCalls_7()
        {
            processor.Reboot();
            webProcessorMock.Verify(w => w.Click(It.IsAny<string>()), Times.Exactly(7));
        }
    }
}
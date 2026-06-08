using System;
using NuciWeb;
using NuciWeb.Automation;
using WirelessRouterRebooter.Service.Models;

namespace WirelessRouterRebooter.Service.Processors
{
    public sealed class TpLinkMR105Processor(IWebProcessor webProcessor) : IRouterProcessor
    {
        public string BrandName => "TP-Link";

        public string ModelName => "MR105";

        public string IpAddress => "192.168.0.1";

        public void LogIn(UserCredentials userCredentials)
        {
            webProcessor.GoToUrl($"http://{IpAddress}/");

            webProcessor.SetText(Select.ById("pc-login-password"), userCredentials.Password);

            webProcessor.Click(Select.ById("pc-login-btn"));

            string ltePanelXpath = Select.ById("lte_panel");
            string confirmButtonXpath = Select.ByXPath("//*[@id='alert-container']/div/div[@class='position-center-left']/div/div[@class='msg-btn-container']/div/div[2]/button");

            webProcessor.WaitForAnyElementToBeVisible(confirmButtonXpath, ltePanelXpath);

            if (webProcessor.IsElementVisible(confirmButtonXpath))
            {
                webProcessor.Click(confirmButtonXpath);
                webProcessor.WaitForAllElementsToBeVisible(ltePanelXpath);
            }
        }

        public void Reboot()
        {
            webProcessor.Click(Select.ByXPath("//*[@id='topReboot']/span[@class='icon']"));
            webProcessor.Click(Select.ByXPath("//*[@id='alert-container']/div/div[@class='position-center-left']/div/div[@class='msg-btn-container']/div/div[2]/button"));
            webProcessor.Wait(TimeSpan.FromSeconds(5));
        }
    }
}

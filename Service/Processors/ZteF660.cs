using System;
using NuciWeb;
using NuciWeb.Automation;
using WirelessRouterRebooter.Service.Models;

namespace WirelessRouterRebooter.Service.Processors
{
    public sealed class ZteF660(IWebProcessor webProcessor) : IRouterProcessor
    {
        public string BrandName => "ZTE";

        public string ModelName => "F660";

        public string IpAddress => "192.168.1.1";

        public void LogIn(UserCredentials userCredentials)
        {
            webProcessor.GoToUrl($"http://{IpAddress}/");

            webProcessor.SetText(Select.ById("Frm_Username"), userCredentials.Username);
            webProcessor.SetText(Select.ById("Frm_Password"), userCredentials.Password);

            webProcessor.Click(Select.ById("LoginId"));
            webProcessor.WaitForElementToDisappear(Select.ById("LoginId"));
        }

        public void Reboot()
        {
            webProcessor.GoToUrl($"http://{IpAddress}/getpage.gch?pid=1002&nextpage=manager_dev_conf_t.gch");
            webProcessor.Click(Select.ById("Submit1"));
            webProcessor.AcceptAlert();
            webProcessor.Wait(TimeSpan.FromSeconds(3));
        }
    }
}

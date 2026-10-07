using System;
using NuciWeb;
using NuciWeb.Automation;
using WirelessRouterRebooter.Service.Models;

namespace WirelessRouterRebooter.Service.Processors
{
    public sealed class ZteF660(
        IWebProcessor webProcessor,
        RouterAccessInfo accessInfo)
        : RouterProcessor("ZTE", "F660", "192.168.1.1", accessInfo)
    {
        public override void LogIn(RouterAccessInfo accessInfo)
        {
            webProcessor.GoToUrl($"http://{IpAddress}/");

            webProcessor.SetText(Select.ById("Frm_Username"), accessInfo.Username);
            webProcessor.SetText(Select.ById("Frm_Password"), accessInfo.Password);

            webProcessor.Click(Select.ById("LoginId"));
            webProcessor.WaitForElementToDisappear(Select.ById("LoginId"));
        }

        public override void Reboot()
        {
            webProcessor.GoToUrl($"http://{IpAddress}/getpage.gch?pid=1002&nextpage=manager_dev_conf_t.gch");
            webProcessor.Click(Select.ById("Submit1"));
            webProcessor.AcceptAlert();
            webProcessor.Wait(TimeSpan.FromSeconds(3));
        }
    }
}

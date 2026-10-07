using NuciWeb;
using NuciWeb.Automation;
using WirelessRouterRebooter.Service.Models;

namespace WirelessRouterRebooter.Service.Processors
{
    public sealed class CompalCH7465VF(
        IWebProcessor webProcessor,
        RouterAccessInfo accessInfo)
        : RouterProcessor("Compal", "CH7465VF", "192.168.0.1", accessInfo)
    {
        public override void LogIn(RouterAccessInfo accessInfo)
        {
            webProcessor.GoToUrl($"http://{IpAddress}/");

            webProcessor.Wait(5000);

            webProcessor.SetText(Select.ByName("loginUsername"), accessInfo.Username);
            webProcessor.SetText(Select.ByName("loginPassword"), accessInfo.Password);

            webProcessor.Click(Select.ById("c_42"));
        }

        public override void Reboot()
        {
            webProcessor.Wait(5000);

            for (int i = 0; i < 3; i++)
            {
                webProcessor.Click(Select.ById("c_mu25"));
                webProcessor.Wait(250);
            }

            webProcessor.Wait(1000);

            for (int i = 0; i < 3; i++)
            {
                webProcessor.Click(Select.ById("c_mu27"));
                webProcessor.Wait(250);
            }

            webProcessor.Click(Select.ById("c_rr14"));
        }
    }
}

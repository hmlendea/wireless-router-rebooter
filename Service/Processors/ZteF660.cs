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

            webProcessor.Wait(5000);

            webProcessor.SetText(Select.ByName("Frm_Username"), userCredentials.Username);
            webProcessor.SetText(Select.ByName("Frm_Password"), userCredentials.Password);

            webProcessor.Click(Select.ById("c_42"));
        }

        public void Reboot()
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

using NuciWeb;
using NuciWeb.Automation;
using WirelessRouterRebooter.Service.Models;

namespace WirelessRouterRebooter.Service.Processors
{
    public sealed class CompalCH7465VF(IWebProcessor webProcessor) : IRouterProcessor
    {
        public string BrandName => "Compal";

        public string ModelName => "CH7465VF";

        public string IpAddress => "192.168.0.1";

        public void LogIn(UserCredentials userCredentials)
        {
            webProcessor.GoToUrl($"http://{IpAddress}/");

            webProcessor.Wait(5000);

            webProcessor.SetText(Select.ByName("loginUsername"), userCredentials.Username);
            webProcessor.SetText(Select.ByName("loginPassword"), userCredentials.Password);

            webProcessor.Click(Select.ById("LoginId"));
            webProcessor.WaitForElementToBeVisible(Select.ById("mmStatu"));
        }

        public void Reboot()
        {
            webProcessor.Click(Select.ById("mmManager"));
            webProcessor.Click(Select.ById("smSysMgr"));
            webProcessor.Click(Select.ById("Submit1"));
        }
    }
}

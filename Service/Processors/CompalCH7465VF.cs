using System;

using NuciWeb;
using OpenQA.Selenium;
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

            webProcessor.SetText(By.Name("loginUsername"), userCredentials.Username);
            webProcessor.SetText(By.Name("loginPassword"), userCredentials.Password);

            webProcessor.Click(By.Id("c_42"));
        }

        public void Reboot()
        {
            webProcessor.Wait(5000);

            webProcessor.Click(By.Id("c_mu25"));

            webProcessor.Wait(1000);
            webProcessor.Click(By.Id("c_mu27"));

            webProcessor.Click(By.Id("c_rr14"));
        }
    }
}

using WirelessRouterRebooter.Service.Models;

namespace WirelessRouterRebooter.Service.Processors
{
    public interface IRouterProcessor
    {
        public string BrandName { get; }

        public string ModelName { get; }

        public string IpAddress { get; }

        public void LogIn(RouterAccessInfo accessInfo);

        public void Reboot();
    }
}

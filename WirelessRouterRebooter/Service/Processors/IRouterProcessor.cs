using WirelessRouterRebooter.Service.Models;

namespace WirelessRouterRebooter.Service.Processors
{
    public interface IRouterProcessor
    {
        string BrandName { get; }

        string ModelName { get; }

        string IpAddress { get; }

        void LogIn(RouterAccessInfo accessInfo);

        void Reboot();
    }
}

using WirelessRouterRebooter.Service.Models;

namespace WirelessRouterRebooter.Service
{
    public interface IBotService
    {
        void Run(RouterAccessInfo accessInfo);
    }
}

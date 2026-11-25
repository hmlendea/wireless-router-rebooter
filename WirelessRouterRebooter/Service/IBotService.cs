using WirelessRouterRebooter.Service.Models;

namespace WirelessRouterRebooter.Service
{
    public interface IBotService
    {
        public void Run(RouterAccessInfo accessInfo);
    }
}

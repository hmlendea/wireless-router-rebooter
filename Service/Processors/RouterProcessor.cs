using WirelessRouterRebooter.Service.Models;

namespace WirelessRouterRebooter.Service.Processors
{
    public abstract class RouterProcessor : IRouterProcessor
    {
        protected RouterProcessor(
            string brandName,
            string modelName,
            string defaultIpAddress,
            RouterAccessInfo accessInfo)
        {
            BrandName = brandName;
            ModelName = modelName;
            IpAddress = string.IsNullOrWhiteSpace(accessInfo?.IpAddress)
                ? defaultIpAddress
                : accessInfo.IpAddress;
        }

        public string BrandName { get; }

        public string ModelName { get; }

        public string IpAddress { get; }

        public abstract void LogIn(RouterAccessInfo accessInfo);

        public abstract void Reboot();
    }
}

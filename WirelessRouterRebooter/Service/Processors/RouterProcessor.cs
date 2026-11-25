using WirelessRouterRebooter.Service.Models;

namespace WirelessRouterRebooter.Service.Processors
{
    public abstract class RouterProcessor : IRouterProcessor
    {
        public string BrandName { get; }

        public string ModelName { get; }

        public string IpAddress { get; }

        protected RouterProcessor(
            string brandName,
            string modelName,
            string defaultIpAddress,
            RouterAccessInfo accessInfo)
        {
            BrandName = brandName;
            ModelName = modelName;
            IpAddress = defaultIpAddress;

            if (!string.IsNullOrWhiteSpace(accessInfo?.IpAddress))
            {
                IpAddress = accessInfo.IpAddress;
            }
        }

        public abstract void LogIn(RouterAccessInfo accessInfo);

        public abstract void Reboot();
    }
}

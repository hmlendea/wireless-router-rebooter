using NuciLog.Core;

namespace WirelessRouterRebooter.Logging
{
    public sealed class MyLogInfoKey : LogInfoKey
    {
        public static LogInfoKey RouterProcessor => new MyLogInfoKey(nameof(RouterProcessor));

        public static LogInfoKey Username => new MyLogInfoKey(nameof(Username));

        public static LogInfoKey BrandName => new MyLogInfoKey(nameof(BrandName));

        public static LogInfoKey ModelName => new MyLogInfoKey(nameof(ModelName));

        public static LogInfoKey IpAddress => new MyLogInfoKey(nameof(IpAddress));

        private MyLogInfoKey(string name) : base(name) { }
    }
}

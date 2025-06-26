using NuciLog.Core;

namespace WirelessRouterRebooter.Logging
{
    public sealed class MyLogInfoKey : LogInfoKey
    {
        MyLogInfoKey(string name) : base(name) { }

        public static LogInfoKey RouterProcessor => new MyLogInfoKey(nameof(RouterProcessor));

        public static LogInfoKey Username => new MyLogInfoKey(nameof(Username));

        public static LogInfoKey RetryDelay => new MyLogInfoKey(nameof(RetryDelay));

        public static LogInfoKey BrandName => new MyLogInfoKey(nameof(BrandName));

        public static LogInfoKey ModelName => new MyLogInfoKey(nameof(ModelName));

        public static LogInfoKey IpAddress => new MyLogInfoKey(nameof(IpAddress));
    }
}
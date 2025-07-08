using NuciLog.Core;

namespace WirelessRouterRebooter.Logging
{
    public sealed class MyOperation : Operation
    {
        MyOperation(string name) : base(name) { }

        public static Operation ParseArguments => new MyOperation(nameof(ParseArguments));

        public static Operation LogIn => new MyOperation(nameof(LogIn));

        public static Operation Reboot => new MyOperation(nameof(Reboot));
    }
}
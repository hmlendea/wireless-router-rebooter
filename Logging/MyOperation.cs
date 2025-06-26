using NuciLog.Core;

namespace WirelessRouterRebooter.Logging
{
    public sealed class MyOperation : Operation
    {
        MyOperation(string name) : base(name) { }

        public static Operation CrashRecovery => new MyOperation(nameof(CrashRecovery));

        public static Operation ConfigurationUpdate => new MyOperation(nameof(ConfigurationUpdate));

        public static Operation ConnectivityTest => new MyOperation(nameof(ConnectivityTest));

        public static Operation BotStart => new MyOperation(nameof(BotStart));

        public static Operation BotStop => new MyOperation(nameof(BotStop));

        public static Operation LogIn => new MyOperation(nameof(LogIn));

        public static Operation Reboot => new MyOperation(nameof(Reboot));
    }
}
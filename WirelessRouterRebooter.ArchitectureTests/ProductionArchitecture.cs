using System.Reflection;

using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using ArchUnitNET.Loader;

using WirelessRouterRebooter.Service;

using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace WirelessRouterRebooter.ArchitectureTests
{
    internal static class ProductionArchitecture
    {
        internal static System.Reflection.Assembly Assembly { get; } = typeof(BotService).Assembly;

        internal static Architecture Model { get; } = new ArchLoader()
            .LoadAssemblies(Assembly)
            .Build();

        internal static IObjectProvider<IType> ProductionTypes { get; } = Types()
            .That()
            .ResideInAssembly(Assembly)
            .As("WirelessRouterRebooter production types");
    }
}

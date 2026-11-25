using System;
using System.Linq;

using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using ArchUnitNET.NUnit;

using NUnit.Framework;

using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace WirelessRouterRebooter.ArchitectureTests
{
    [TestFixture]
    public sealed class ArchitectureBoundaryTests
    {
        private static string ConfigurationNamespace => "WirelessRouterRebooter.Configuration";

        private static string ModelsNamespace => "WirelessRouterRebooter.Service.Models";

        private static string ProcessorsNamespace => "WirelessRouterRebooter.Service.Processors";

        [Test]
        public void GivenProductionAssembly_WhenLoaded_ThenEveryDocumentedNamespaceExists()
        {
            string[] namespaces = ProductionArchitecture.Model.Namespaces
                .Select(definition => definition.FullName)
                .ToArray();

            Assert.That(namespaces, Is.Not.Empty);
            Assert.That(namespaces, Does.Contain("WirelessRouterRebooter"));
            Assert.That(namespaces, Does.Contain(ConfigurationNamespace));
            Assert.That(namespaces, Does.Contain("WirelessRouterRebooter.Logging"));
            Assert.That(namespaces, Does.Contain("WirelessRouterRebooter.Service"));
            Assert.That(namespaces, Does.Contain(ModelsNamespace));
            Assert.That(namespaces, Does.Contain(ProcessorsNamespace));
        }

        [Test]
        public void GivenEachDocumentedNamespace_WhenSelected_ThenItContainsAtLeastOneType()
        {
            string[] namespaces =
            [
                ConfigurationNamespace,
                "WirelessRouterRebooter.Logging",
                "WirelessRouterRebooter.Service",
                ModelsNamespace,
                ProcessorsNamespace
            ];

            foreach (string namespaceName in namespaces)
            {
                IObjectProvider<IType> types = Types().That().ResideInNamespace(namespaceName);

                Assert.That(types.GetObjects(ProductionArchitecture.Model), Is.Not.Empty, $"The namespace '{namespaceName}' must declare at least one type.");
            }
        }

        [Test]
        public void GivenConfigurationTypes_WhenChecked_ThenTheyDoNotDependOnServices()
        {
            IArchRule rule = Types()
                .That().ResideInNamespace(ConfigurationNamespace)
                .Should().NotDependOnAny(Types().That().ResideInNamespace("WirelessRouterRebooter.Service"))
                .Because("configuration models are input models and must not depend on services");

            rule.Check(ProductionArchitecture.Model);
        }

        [Test]
        public void GivenModels_WhenChecked_ThenTheyDoNotDependOnProcessors()
        {
            IArchRule rule = Types()
                .That().ResideInNamespace(ModelsNamespace)
                .Should().NotDependOnAny(Types().That().ResideInNamespace(ProcessorsNamespace))
                .Because("models must not depend on browser or router implementations");

            rule.Check(ProductionArchitecture.Model);
        }

        [Test]
        public void GivenProductionTypes_WhenChecked_ThenTheyDoNotDependOnTheEntryPoint()
        {
            IArchRule rule = Types()
                .That().Are(ProductionArchitecture.ProductionTypes)
                .And().DoNotHaveFullName("WirelessRouterRebooter.Program")
                .Should().NotDependOnAny(Types().That().HaveFullName("WirelessRouterRebooter.Program"))
                .Because("the entry point is a composition root and must stay a leaf consumer");

            rule.Check(ProductionArchitecture.Model);
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenInternalsVisibleToIsAbsent()
        {
            foreach (string source in EmbeddedSources.ProductionSources)
            {
                Assert.That(EmbeddedSources.ReadProduction(source), Does.Not.Contain("InternalsVisibleTo"), $"{source}: production code must not expose internals to tests.");
            }
        }
    }
}

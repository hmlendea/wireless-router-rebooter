using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using NUnit.Framework;

namespace WirelessRouterRebooter.ArchitectureTests
{
    [TestFixture]
    public sealed class NamespaceAndUsingConventionTests
    {
        [Test]
        public void GivenProductionSources_WhenChecked_ThenNamespacesAndUsingsMatchRules()
        {
            foreach (string source in EmbeddedSources.ProductionSources)
            {
                NamespaceDeclarationSyntax[] namespaces = EmbeddedSources.ParseProduction(source).GetRoot()
                    .DescendantNodes().OfType<NamespaceDeclarationSyntax>().ToArray();
                Assert.That(namespaces, Has.Length.EqualTo(1), $"{source}: exactly one namespace is required.");
                Assert.That(namespaces[0].Name.ToString(), Is.EqualTo(EmbeddedSources.GetExpectedNamespace(source)), $"{source}: namespace must mirror its folder.");
                Assert.That(namespaces[0].Name.ToString(), Does.Not.EndWith(".Interfaces"), $"{source}: Interfaces namespaces are forbidden.");

                string[] usingNames = EmbeddedSources.ParseProduction(source).GetRoot().DescendantNodes()
                    .OfType<UsingDirectiveSyntax>().Select(directive => directive.Name?.ToString() ?? string.Empty).ToArray();
                string[] systemUsings = usingNames.Where(name => name.StartsWith("System", StringComparison.Ordinal)).ToArray();
                Assert.That(systemUsings, Is.EqualTo(systemUsings.OrderBy(name => name, StringComparer.Ordinal).ToArray()), $"{source}: System usings must be alphabetised.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenNoInlineFullyQualifiedTypesOrGlobalUsingsExist()
        {
            foreach (string source in EmbeddedSources.ProductionSources)
            {
                string text = EmbeddedSources.ReadProduction(source);
                Assert.That(text, Does.Not.Match(@"(?m)^\s*global\s+using\b"), $"{source}: global usings are forbidden.");
                Assert.That(text, Does.Not.Match(@"(?m)^(?!\s*using\s)(?!\s*namespace\s).*\b(?:System|Microsoft|NuciWeb|NuciLog|NuciCLI)\.[A-Za-z_]"), $"{source}: inline fully qualified types are forbidden.");
            }
        }
    }
}

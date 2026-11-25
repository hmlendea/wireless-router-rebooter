using System;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using NUnit.Framework;

namespace WirelessRouterRebooter.ArchitectureTests
{
    [TestFixture]
    public sealed class DocumentationConventionTests
    {
        [Test]
        public void GivenProductionSources_WhenChecked_ThenDocumentationBlocksAreWellFormed()
        {
            foreach (string source in EmbeddedSources.ProductionSources)
            {
                string text = EmbeddedSources.ReadProduction(source);
                Assert.That(text, Does.Not.Match(@"///\s*<summary>\s*///\s*</summary>"), $"{source}: summaries must not be empty.");
            }
        }

        [Test]
        public void GivenProductionProject_WhenChecked_ThenNuGetDocumentationRulesAreScopedToPackableProjects()
        {
            string project = EmbeddedSources.ReadProjectFile("WirelessRouterRebooter/WirelessRouterRebooter.csproj");

            Assert.That(project, Does.Contain("<IsPackable>false</IsPackable>"));
        }
    }
}

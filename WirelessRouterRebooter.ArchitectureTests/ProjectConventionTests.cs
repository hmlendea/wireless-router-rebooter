using System;
using System.Linq;
using System.Xml.Linq;

using NUnit.Framework;

namespace WirelessRouterRebooter.ArchitectureTests
{
    [TestFixture]
    public sealed class ProjectConventionTests
    {
        [Test]
        public void GivenProductionProject_WhenRead_ThenRequiredPropertiesAreDeclared()
        {
            XDocument project = XDocument.Parse(EmbeddedSources.ReadProjectFile("WirelessRouterRebooter/WirelessRouterRebooter.csproj"));
            XElement properties = project.Descendants("PropertyGroup").Single();

            Assert.That(properties.Element("TargetFramework")?.Value, Is.EqualTo("net10.0"));
            Assert.That(properties.Element("ImplicitUsings")?.Value, Is.EqualTo("disable"));
            Assert.That(properties.Element("LangVersion")?.Value, Is.EqualTo("latest"));
            Assert.That(properties.Element("Nullable")?.Value, Is.EqualTo("enable"));
            Assert.That(properties.Element("IsPackable")?.Value, Is.EqualTo("false"));
        }

        [Test]
        public void GivenTestProjects_WhenRead_ThenRequiredTestPropertiesAreDeclared()
        {
            XDocument project = XDocument.Parse(EmbeddedSources.ReadProjectFile("WirelessRouterRebooter.UnitTests/WirelessRouterRebooter.UnitTests.csproj"));
            XElement properties = project.Descendants("PropertyGroup").Single();

            Assert.That(properties.Element("TargetFramework")?.Value, Is.EqualTo("net10.0"));
            Assert.That(properties.Element("ImplicitUsings")?.Value, Is.EqualTo("disable"));
            Assert.That(properties.Element("LangVersion")?.Value, Is.EqualTo("latest"));
            Assert.That(properties.Element("Nullable")?.Value, Is.EqualTo("enable"));
            Assert.That(properties.Element("IsPackable")?.Value, Is.EqualTo("false"));
            Assert.That(properties.Element("IsTestProject")?.Value, Is.EqualTo("true"));
            Assert.That(properties.Element("ExcludeFromCodeCoverage")?.Value, Is.EqualTo("true"));
        }

        [Test]
        public void GivenProjectFiles_WhenRead_ThenPackageReferencesAreAlphabetised()
        {
            string[] projectPaths =
            [
                "WirelessRouterRebooter/WirelessRouterRebooter.csproj",
                "WirelessRouterRebooter.UnitTests/WirelessRouterRebooter.UnitTests.csproj"
            ];

            foreach (string projectPath in projectPaths)
            {
                XDocument project = XDocument.Parse(EmbeddedSources.ReadProjectFile(projectPath));
                string[] packages = project.Descendants("PackageReference")
                    .Select(reference => reference.Attribute("Include")?.Value ?? string.Empty)
                    .ToArray();

                Assert.That(packages, Is.EqualTo(packages.OrderBy(package => package, StringComparer.Ordinal).ToArray()), $"{projectPath}: package references must be alphabetised.");
            }
        }

        [Test]
        public void GivenSolutionFile_WhenRead_ThenProjectsAreOrderedByRole()
        {
            XDocument solution = XDocument.Parse(EmbeddedSources.ReadProjectFile("WirelessRouterRebooter.slnx"));
            string[] paths = solution.Descendants("Project")
                .Select(project => project.Attribute("Path")?.Value ?? string.Empty)
                .ToArray();

            Assert.That(paths, Has.Length.EqualTo(4));
            Assert.That(paths[0], Does.Contain("WirelessRouterRebooter.csproj"));
            Assert.That(paths[1], Does.Contain("UnitTests.csproj"));
            Assert.That(paths[2], Does.Contain("ArchitectureTests.csproj"));
            Assert.That(paths[3], Does.Contain("IntegrationTests.csproj"));
            Assert.That(paths.Any(path => path.Contains("/src/", StringComparison.Ordinal)), Is.False);
        }
    }
}

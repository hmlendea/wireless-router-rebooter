using System;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using NUnit.Framework;

namespace WirelessRouterRebooter.ArchitectureTests
{
    [TestFixture]
    public sealed class InterfaceConventionTests
    {
        [Test]
        public void GivenProductionInterfaces_WhenChecked_ThenNamesAndAccessibilityAreValid()
        {
            foreach (string source in EmbeddedSources.ProductionSources)
            {
                foreach (InterfaceDeclarationSyntax declaration in EmbeddedSources.ParseProduction(source).GetRoot().DescendantNodes().OfType<InterfaceDeclarationSyntax>())
                {
                    Assert.That(declaration.Identifier.ValueText, Does.StartWith("I"), $"{source}: interface names must start with I.");
                    Assert.That(char.IsUpper(declaration.Identifier.ValueText[1]), Is.True, $"{source}: interface names must use PascalCase.");
                    Assert.That(declaration.Modifiers.Any(modifier => modifier.ValueText == "public"), Is.True, $"{source}: interfaces must declare public accessibility.");
                }
            }
        }

        [Test]
        public void GivenProductionInterfaces_WhenChecked_ThenMembersDeclarePublicAccessibility()
        {
            foreach (string source in EmbeddedSources.ProductionSources)
            {
                foreach (InterfaceDeclarationSyntax declaration in EmbeddedSources.ParseProduction(source).GetRoot().DescendantNodes().OfType<InterfaceDeclarationSyntax>())
                {
                    foreach (MemberDeclarationSyntax member in declaration.Members)
                    {
                        Assert.That(member.Modifiers.Any(modifier => modifier.ValueText == "public"), Is.True, $"{source}: interface member '{member}' must explicitly declare public accessibility.");
                    }
                }
            }
        }
    }
}

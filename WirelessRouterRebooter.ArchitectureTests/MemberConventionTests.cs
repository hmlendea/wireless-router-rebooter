using System;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using NUnit.Framework;

namespace WirelessRouterRebooter.ArchitectureTests
{
    [TestFixture]
    public sealed class MemberConventionTests
    {
        [Test]
        public void GivenProductionSources_WhenChecked_ThenPublicAndProtectedIdentifiersUsePascalCase()
        {
            foreach (string source in EmbeddedSources.ProductionSources)
            {
                SyntaxNode root = EmbeddedSources.ParseProduction(source).GetRoot();
                foreach (MemberDeclarationSyntax member in root.DescendantNodes().OfType<MemberDeclarationSyntax>())
                {
                    bool isPublicOrProtected = member.Modifiers.Any(modifier => modifier.ValueText == "public" || modifier.ValueText == "protected");
                    if (!isPublicOrProtected)
                    {
                        continue;
                    }

                    string name = member switch
                    {
                        MethodDeclarationSyntax method => method.Identifier.ValueText,
                        PropertyDeclarationSyntax property => property.Identifier.ValueText,
                        EventDeclarationSyntax eventDeclaration => eventDeclaration.Identifier.ValueText,
                        FieldDeclarationSyntax field => field.Declaration.Variables.First().Identifier.ValueText,
                        TypeDeclarationSyntax type => type.Identifier.ValueText,
                        _ => string.Empty
                    };

                    if (name.Length == 0)
                    {
                        continue;
                    }

                    Assert.That(char.IsUpper(name[0]), Is.True, $"{source}: public/protected member '{name}' must use PascalCase.");
                }
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenBooleansUseApprovedPrefixes()
        {
            foreach (string source in EmbeddedSources.ProductionSources)
            {
                foreach (PropertyDeclarationSyntax property in EmbeddedSources.ParseProduction(source).GetRoot().DescendantNodes().OfType<PropertyDeclarationSyntax>())
                {
                    if (!property.Type.ToString().Equals("bool", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    Assert.That(property.Identifier.ValueText, Does.Match("^(Is|Has|Does|Are|Was|Were)"), $"{source}: boolean '{property.Identifier.ValueText}' must use an approved prefix.");
                }
            }
        }

        [Test]
        public void GivenProductionTypes_WhenChecked_ThenConcreteClassesAreSealedUnlessAbstract()
        {
            foreach (string source in EmbeddedSources.ProductionSources)
            {
                foreach (ClassDeclarationSyntax declaration in EmbeddedSources.ParseProduction(source).GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>())
                {
                    bool isAbstract = declaration.Modifiers.Any(modifier => modifier.ValueText == "abstract");
                    bool isSealed = declaration.Modifiers.Any(modifier => modifier.ValueText == "sealed");
                    Assert.That(isAbstract || isSealed, Is.True, $"{source}: class '{declaration.Identifier.ValueText}' must be sealed or abstract.");
                }
            }
        }
    }
}

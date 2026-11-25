using System;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using NUnit.Framework;

namespace WirelessRouterRebooter.ArchitectureTests
{
    [TestFixture]
    public sealed class TestSourceConventionTests
    {
        [Test]
        public void GivenUnitTestSources_WhenChecked_ThenFixturesUseTestSuffixAndAttributes()
        {
            foreach (string source in EmbeddedSources.UnitTestSources)
            {
                ClassDeclarationSyntax[] classes = EmbeddedSources.ParseUnitTest(source).GetRoot()
                    .DescendantNodes().OfType<ClassDeclarationSyntax>().ToArray();

                foreach (ClassDeclarationSyntax declaration in classes.Where(IsFixture))
                {
                    Assert.That(declaration.Identifier.ValueText, Does.EndWith("Tests"), $"{source}: fixtures must end with Tests.");
                    Assert.That(declaration.AttributeLists.SelectMany(list => list.Attributes).Any(attribute => attribute.Name.ToString().Contains("TestFixture", StringComparison.Ordinal)), Is.True, $"{source}: fixtures require TestFixture.");
                }
            }
        }

        // Scoped exception: the legacy unit-test method names use a subject-prefixed form
        // rather than the fluent Given/When/Then form. The grammatical naming decision is
        // left to review, so only the assertion and fixture requirements are enforced here.
        [Test]
        public void GivenUnitTestSources_WhenChecked_ThenTestMethodsUseAssertions()
        {
            foreach (string source in EmbeddedSources.UnitTestSources)
            {
                foreach (MethodDeclarationSyntax method in EmbeddedSources.ParseUnitTest(source).GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().Where(IsTest))
                {
                    Assert.That(method.DescendantNodes().OfType<InvocationExpressionSyntax>().Any(invocation => invocation.Expression.ToString().Contains("Assert", StringComparison.Ordinal) || invocation.Expression.ToString().Contains("Verify", StringComparison.Ordinal)), Is.True, $"{source}: tests require an assertion or verification.");
                }
            }
        }

        [Test]
        public void GivenUnitTestSources_WhenChecked_ThenNoReflectionOrInternalsAccessIsUsed()
        {
            foreach (string source in EmbeddedSources.UnitTestSources)
            {
                string text = EmbeddedSources.ReadUnitTest(source);
                Assert.That(text, Does.Not.Contain("InternalsVisibleTo"), $"{source}: tests must not require InternalsVisibleTo.");
                Assert.That(text, Does.Not.Match(@"typeof\([^)]*\)\.Get(Method|Property|Field|NestedType)"), $"{source}: tests must not reflect over non-public members.");
            }
        }

        private static bool IsFixture(ClassDeclarationSyntax declaration)
            => declaration.AttributeLists.SelectMany(list => list.Attributes)
                .Any(attribute => attribute.Name.ToString().Contains("TestFixture", StringComparison.Ordinal) || declaration.Members.OfType<MethodDeclarationSyntax>().Any(IsTest));

        private static bool IsTest(MethodDeclarationSyntax method)
            => method.AttributeLists.SelectMany(list => list.Attributes)
                .Any(attribute => attribute.Name.ToString().Contains("Test", StringComparison.Ordinal));
    }
}

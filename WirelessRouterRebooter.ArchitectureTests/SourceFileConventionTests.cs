using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using NUnit.Framework;

namespace WirelessRouterRebooter.ArchitectureTests
{
    [TestFixture]
    public sealed class SourceFileConventionTests
    {
        [Test]
        public void GivenProductionSources_WhenChecked_ThenEveryFileHasOneMatchingType()
        {
            foreach (string source in GetProductionSources())
            {
                IReadOnlyList<string> declaredTypes = GetDeclaredTypeNames(source);

                Assert.That(declaredTypes, Has.Count.EqualTo(1), $"{source}: exactly one type is required.");
                Assert.That(declaredTypes[0], Is.EqualTo(GetFileStem(source)), $"{source}: type and file names must match.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenNamespacesMatchFolders()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxTree tree = ParseProductionSource(source);
                string expectedNamespace = GetExpectedNamespace(source);
                string[] namespaces = tree.GetRoot().DescendantNodes().OfType<NamespaceDeclarationSyntax>()
                    .Select(declaration => declaration.Name.ToString()).ToArray();

                Assert.That(namespaces, Has.Length.EqualTo(1), $"{source}: one block namespace is required.");
                Assert.That(namespaces[0], Is.EqualTo(expectedNamespace), $"{source}: namespace must match its folder.");
                Assert.That(tree.GetRoot().DescendantNodes().OfType<FileScopedNamespaceDeclarationSyntax>(), Is.Empty);
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenNoNestedTypesOrTopLevelStatementsExist()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseProductionSource(source).GetRoot();

                Assert.That(root.DescendantNodes().OfType<BaseTypeDeclarationSyntax>()
                    .Any(declaration => declaration.Parent is TypeDeclarationSyntax), Is.False, $"{source}: nested types are forbidden.");
                Assert.That(root.DescendantNodes().OfType<GlobalStatementSyntax>(), Is.Empty, $"{source}: top-level statements are forbidden.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenControlFlowUsesBraces()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseProductionSource(source).GetRoot();
                IEnumerable<StatementSyntax> unbraced = root.DescendantNodes().OfType<IfStatementSyntax>().Where(statement => statement.Statement is not BlockSyntax).Select(statement => statement.Statement)
                    .Concat(root.DescendantNodes().OfType<ForStatementSyntax>().Where(statement => statement.Statement is not BlockSyntax).Select(statement => statement.Statement))
                    .Concat(root.DescendantNodes().OfType<ForEachStatementSyntax>().Where(statement => statement.Statement is not BlockSyntax).Select(statement => statement.Statement))
                    .Concat(root.DescendantNodes().OfType<WhileStatementSyntax>().Where(statement => statement.Statement is not BlockSyntax).Select(statement => statement.Statement));

                Assert.That(unbraced, Is.Empty, $"{source}: control-flow bodies require braces.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenParameterLimitsAndParameterFormsAreValid()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseProductionSource(source).GetRoot();
                IEnumerable<ParameterSyntax> parameters = root.DescendantNodes().OfType<ParameterSyntax>();

                Assert.That(root.DescendantNodes().OfType<BaseMethodDeclarationSyntax>().Any(method => method.ParameterList.Parameters.Count > 7), Is.False, $"{source}: methods may have at most seven parameters.");
                Assert.That(root.DescendantNodes().OfType<AnonymousFunctionExpressionSyntax>().Any(lambda => GetLambdaParameterCount(lambda) > 7), Is.False, $"{source}: lambdas may have at most seven parameters.");
                Assert.That(parameters.Any(parameter => parameter.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.RefKeyword) || modifier.IsKind(SyntaxKind.OutKeyword))), Is.False, $"{source}: ref and out parameters are forbidden.");
                Assert.That(parameters.Any(parameter => parameter.Default is not null), Is.False, $"{source}: optional parameters are forbidden.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenCompilerAndPreviewFormsAreAbsent()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseProductionSource(source).GetRoot();

                Assert.That(root.DescendantNodes().OfType<RecordDeclarationSyntax>(), Is.Empty, $"{source}: records are not allowed.");
                Assert.That(root.DescendantNodes().OfType<AccessorDeclarationSyntax>().Any(accessor => accessor.IsKind(SyntaxKind.InitAccessorDeclaration)), Is.False, $"{source}: init accessors are not allowed.");
                Assert.That(root.DescendantNodes().OfType<TypeDeclarationSyntax>().Any(type => type.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PartialKeyword))), Is.False, $"{source}: partial types require an approved generator exception.");
                Assert.That(ReadProductionSource(source), Does.Not.Match(@"(?m)^\s*#nullable\s+"), $"{source}: nullable directives are forbidden.");
                Assert.That(ReadProductionSource(source), Does.Not.Match(@"(?m)^\s*#(end)?region\b"), $"{source}: regions are forbidden.");
            }
        }

        private static IReadOnlyList<string> GetProductionSources()
        {
            IReadOnlyList<string> sources = ArchitectureResources.GetResourcePaths(ArchitectureResources.ProductionSourcePrefix);
            Assert.That(sources, Is.Not.Empty, "No production sources were embedded.");
            return sources;
        }

        private static SyntaxTree ParseProductionSource(string source)
            => CSharpSyntaxTree.ParseText(ReadProductionSource(source), path: source);

        private static string ReadProductionSource(string source)
            => ArchitectureResources.ReadResource(ArchitectureResources.ProductionSourcePrefix, source);

        private static IReadOnlyList<string> GetDeclaredTypeNames(string source)
        {
            SyntaxNode root = ParseProductionSource(source).GetRoot();
            IEnumerable<string> types = root.DescendantNodes().OfType<BaseTypeDeclarationSyntax>().Select(type => type.Identifier.ValueText);
            IEnumerable<string> delegates = root.DescendantNodes().OfType<DelegateDeclarationSyntax>().Select(declaration => declaration.Identifier.ValueText);
            return types.Concat(delegates).ToArray();
        }

        private static string GetFileStem(string source)
        {
            string fileName = source.Substring(source.LastIndexOf('/') + 1);
            return fileName.Substring(0, fileName.LastIndexOf('.'));
        }

        private static string GetExpectedNamespace(string source)
        {
            int separator = source.LastIndexOf('/');
            string folder = separator < 0 ? string.Empty : source.Substring(0, separator).Replace('/', '.');
            return string.IsNullOrEmpty(folder) ? "WirelessRouterRebooter" : $"WirelessRouterRebooter.{folder}";
        }

        private static int GetLambdaParameterCount(AnonymousFunctionExpressionSyntax lambda)
        {
            ParenthesizedLambdaExpressionSyntax? parenthesized = lambda as ParenthesizedLambdaExpressionSyntax;
            return parenthesized is null ? 1 : parenthesized.ParameterList.Parameters.Count;
        }
    }
}

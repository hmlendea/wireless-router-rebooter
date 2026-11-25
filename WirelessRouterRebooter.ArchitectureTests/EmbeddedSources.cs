using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using NUnit.Framework;

namespace WirelessRouterRebooter.ArchitectureTests
{
    internal static class EmbeddedSources
    {
        internal static IReadOnlyList<string> ProductionSources
            => RequireNonEmpty(
                ArchitectureResources.GetResourcePaths(ArchitectureResources.ProductionSourcePrefix),
                "No production sources were embedded.");

        internal static IReadOnlyList<string> UnitTestSources
            => RequireNonEmpty(
                ArchitectureResources.GetResourcePaths(ArchitectureResources.UnitTestSourcePrefix),
                "No unit-test sources were embedded.");

        internal static string ReadProduction(string source)
            => ArchitectureResources.ReadResource(ArchitectureResources.ProductionSourcePrefix, source);

        internal static string ReadUnitTest(string source)
            => ArchitectureResources.ReadResource(ArchitectureResources.UnitTestSourcePrefix, source);

        internal static string ReadProjectFile(string relativePath)
            => ArchitectureResources.ReadResource(ArchitectureResources.ProjectFilePrefix, relativePath);

        internal static SyntaxTree ParseProduction(string source)
            => CSharpSyntaxTree.ParseText(ReadProduction(source), path: source);

        internal static SyntaxTree ParseUnitTest(string source)
            => CSharpSyntaxTree.ParseText(ReadUnitTest(source), path: source);

        internal static string GetStem(string source)
        {
            string fileName = source.Substring(source.LastIndexOf('/') + 1);
            return fileName.Substring(0, fileName.LastIndexOf('.'));
        }

        internal static string GetExpectedNamespace(string source)
        {
            int separator = source.LastIndexOf('/');

            if (separator < 0)
            {
                return "WirelessRouterRebooter";
            }

            return $"WirelessRouterRebooter.{source.Substring(0, separator).Replace('/', '.')}";
        }

        private static IReadOnlyList<string> RequireNonEmpty(IReadOnlyList<string> paths, string message)
        {
            Assert.That(paths, Is.Not.Empty, message);
            return paths;
        }
    }
}

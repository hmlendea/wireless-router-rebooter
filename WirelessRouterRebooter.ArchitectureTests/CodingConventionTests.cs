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
    public sealed class CodingConventionTests
    {
        [Test]
        public void GivenProductionSources_WhenChecked_ThenForbiddenSyntaxIsAbsent()
        {
            foreach (string source in EmbeddedSources.ProductionSources)
            {
                SyntaxNode root = EmbeddedSources.ParseProduction(source).GetRoot();
                string text = EmbeddedSources.ReadProduction(source);

                Assert.That(root.DescendantNodes().OfType<VariableDeclarationSyntax>().Any(declaration => declaration.Type.IsVar), Is.False, $"{source}: var is forbidden.");
                Assert.That(root.DescendantNodes().OfType<ConditionalExpressionSyntax>(), Is.Empty, $"{source}: ternaries are forbidden.");
                Assert.That(root.DescendantNodes().OfType<BinaryExpressionSyntax>().Any(expression => expression.IsKind(SyntaxKind.CoalesceExpression)), Is.False, $"{source}: ?? is forbidden.");
                Assert.That(root.DescendantNodes().Any(node => node.IsKind(SyntaxKind.TupleType) || node.IsKind(SyntaxKind.TupleExpression)), Is.False, $"{source}: tuples are forbidden.");
                Assert.That(root.DescendantNodes().OfType<FieldDeclarationSyntax>().Any(field => field.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.ConstKeyword))), Is.False, $"{source}: const fields are forbidden.");
                Assert.That(text, Does.Not.Match(@"\bSystem\.(String|Int32|Int64|Boolean|Object|Byte|Char|Double|Single|Decimal|DateTime|Exception|Guid|Convert|Console|Environment|Math)\b"), $"{source}: use imports instead of inline BCL names.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenUsingAndCommentConventionsAreSatisfied()
        {
            foreach (string source in EmbeddedSources.ProductionSources)
            {
                SyntaxTree tree = EmbeddedSources.ParseProduction(source);
                UsingDirectiveSyntax[] usings = tree.GetRoot().DescendantNodes().OfType<UsingDirectiveSyntax>().ToArray();
                Assert.That(usings.Any(usingDirective => usingDirective.GlobalKeyword.IsKind(SyntaxKind.GlobalKeyword) || usingDirective.StaticKeyword.IsKind(SyntaxKind.StaticKeyword)), Is.False, $"{source}: global and static usings are forbidden.");

                string[] lines = EmbeddedSources.ReadProduction(source).Split('\n');
                for (int index = 0; index < lines.Length; index++)
                {
                    string line = lines[index].TrimStart();
                    if (!line.StartsWith("//", StringComparison.Ordinal) || line.StartsWith("///", StringComparison.Ordinal))
                    {
                        continue;
                    }

                    string comment = line.Substring(2);
                    Assert.That(comment.Length == 0 || comment.StartsWith(' '), Is.True, $"{source}:{index + 1}: comments need a space.");
                    if (comment.Trim().Length > 0)
                    {
                        Assert.That(char.IsUpper(comment.TrimStart()[0]), Is.True, $"{source}:{index + 1}: comments need an uppercase start.");
                        Assert.That(comment.TrimEnd(), Does.EndWith("."), $"{source}:{index + 1}: comments need a full stop.");
                    }
                }
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenFormattingIsConsistent()
        {
            foreach (string source in EmbeddedSources.ProductionSources)
            {
                string text = EmbeddedSources.ReadProduction(source);
                Assert.That(text, Does.Not.Contain("\r"), $"{source}: LF line endings are required.");
                Assert.That(text, Does.Not.Match(@"\n[ \t]*\n[ \t]*\n"), $"{source}: consecutive blank lines are forbidden.");
                Assert.That(text, Does.Not.Match(@"[ \t]+\n"), $"{source}: trailing whitespace is forbidden.");
                Assert.That(text, Does.Not.Match(@"(?m)^\s*\t"), $"{source}: tabs are forbidden.");
                Assert.That(text.EndsWith('\n'), Is.True, $"{source}: files need a terminating LF.");
                Assert.That(text.EndsWith("\n\n", StringComparison.Ordinal), Is.False, $"{source}: files need one terminating LF.");
                Assert.That(text, Does.Not.Match(@"\n[ \t]*\n[ \t]*[}\])]"), $"{source}: blank lines before delimiters are forbidden.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenAccessAndNamingRulesAreSatisfied()
        {
            foreach (string source in EmbeddedSources.ProductionSources)
            {
                SyntaxNode root = EmbeddedSources.ParseProduction(source).GetRoot();
                Assert.That(root.DescendantNodes().OfType<TypeDeclarationSyntax>().Any(type => !type.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PublicKeyword) || modifier.IsKind(SyntaxKind.InternalKeyword) || modifier.IsKind(SyntaxKind.PrivateKeyword) || modifier.IsKind(SyntaxKind.ProtectedKeyword))), Is.False, $"{source}: type accessibility must be explicit.");
                Assert.That(root.DescendantNodes().OfType<MethodDeclarationSyntax>().Any(method => !method.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PublicKeyword) || modifier.IsKind(SyntaxKind.InternalKeyword) || modifier.IsKind(SyntaxKind.PrivateKeyword) || modifier.IsKind(SyntaxKind.ProtectedKeyword))), Is.False, $"{source}: method accessibility must be explicit.");
                Assert.That(root.DescendantNodes().OfType<FieldDeclarationSyntax>().SelectMany(field => field.Declaration.Variables).Any(variable => variable.Identifier.ValueText.StartsWith("_", StringComparison.Ordinal)), Is.False, $"{source}: private fields may not use underscore prefixes.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenProjectStyleRequirementsAreSatisfied()
        {
            foreach (string source in EmbeddedSources.ProductionSources)
            {
                SyntaxNode root = EmbeddedSources.ParseProduction(source).GetRoot();
                Assert.That(root.DescendantNodes().OfType<IfStatementSyntax>().Any(statement => statement.Statement is not BlockSyntax), Is.False, $"{source}: if bodies need braces.");
                Assert.That(root.DescendantNodes().OfType<ForStatementSyntax>().Any(statement => statement.Statement is not BlockSyntax), Is.False, $"{source}: for bodies need braces.");
                Assert.That(root.DescendantNodes().OfType<ForEachStatementSyntax>().Any(statement => statement.Statement is not BlockSyntax), Is.False, $"{source}: foreach bodies need braces.");
                Assert.That(root.DescendantNodes().OfType<WhileStatementSyntax>().Any(statement => statement.Statement is not BlockSyntax), Is.False, $"{source}: while bodies need braces.");
                Assert.That(root.DescendantNodes().OfType<MethodDeclarationSyntax>().Any(method => method.ParameterList.Parameters.Count > 7), Is.False, $"{source}: methods may have at most seven parameters.");
            }
        }
    }
}

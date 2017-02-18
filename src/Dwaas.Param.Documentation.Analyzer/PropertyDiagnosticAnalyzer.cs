using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dwaas.Param.Documentation.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class PropertyDiagnosticAnalyzer : DiagnosticAnalyzer
    {
        private static readonly LocalizableString Title =
            new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));

        private static readonly LocalizableString MessageFormat =
            new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));

        private static readonly LocalizableString Description =
            new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(Constants.DiagnosticIdProperty, Title, MessageFormat,
            Constants.DiagnosticIdProperty, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(VerifyPropertyDescription, SyntaxKind.PropertyDeclaration);
        }

        private void VerifyPropertyDescription(SyntaxNodeAnalysisContext context)
        {
            var property = context.Node as PropertyDeclarationSyntax;
            if (property == null || !property.HasStructuredTrivia || !Lookup.Load(context.Options.AdditionalFiles))
            {
                return;
            }

            var xmlTrivia = property.GetLeadingTrivia()
                .Select(i => i.GetStructure())
                .OfType<DocumentationCommentTriviaSyntax>()
                .FirstOrDefault();
            if (xmlTrivia == null)
            {
                return;
            }

            var summary = xmlTrivia.ChildNodes()
                .OfType<XmlElementSyntax>()
                .FirstOrDefault(i => i.StartTag.Name.ToString().Equals(Constants.DiagnosticTagSummary));

            string name = property.Identifier.Text;
            string description;
            if (summary != null && !string.IsNullOrWhiteSpace(name) && Lookup.TryGetValue(name, out description))
            {
                if (summary.Content.ToString() != description)
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        Rule,
                        summary.GetLocation(),
                        (new Dictionary<string, string>()
                        {
                            {Constants.DiagnosticName, name},
                            {Constants.DiagnosticDescription, description}
                        }).ToImmutableDictionary(),
                        name,
                        description));
                }
            }
        }
    }
}

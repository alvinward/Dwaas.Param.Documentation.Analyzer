using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dwaas.Param.Documentation.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ParamDiagnosticAnalyzer : DiagnosticAnalyzer
    {
        private static readonly LocalizableString Title =
            new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));

        private static readonly LocalizableString MessageFormat =
            new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));

        private static readonly LocalizableString Description =
            new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(Constants.DiagnosticIdParam, Title, MessageFormat,
            Constants.DiagnosticIdParam, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(VerifyParamDescription, SyntaxKind.MethodDeclaration);
        }

        private void VerifyParamDescription(SyntaxNodeAnalysisContext context)
        {
            var method = context.Node as MethodDeclarationSyntax;
            if (method == null || !method.HasStructuredTrivia || !Lookup.Load(context.Options.AdditionalFiles))
            {
                return;
            }

            var xmlTrivia = method.GetLeadingTrivia()
                .Select(i => i.GetStructure())
                .OfType<DocumentationCommentTriviaSyntax>()
                .FirstOrDefault();
            if (xmlTrivia == null)
            {
                return;
            }

            var allParams = xmlTrivia.ChildNodes()
                .OfType<XmlElementSyntax>()
                .Where(i => i.StartTag.Name.ToString().Equals(Constants.DiagnosticTagParam)).ToArray();

            foreach (var param in allParams)
            {
                string name = param.StartTag.Attributes.OfType<XmlNameAttributeSyntax>().FirstOrDefault()?.Identifier?.ToString();
                string description;
                if (!string.IsNullOrWhiteSpace(name) && Lookup.TryGetValue(name, out description))
                {
                    if (param.Content.ToString() != description)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(
                            Rule,
                            param.GetLocation(),
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
}

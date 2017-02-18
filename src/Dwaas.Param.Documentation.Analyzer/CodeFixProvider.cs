using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Dwaas.Param.Documentation.Analyzer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ParamDocumentationCodeFixProvider)), Shared]
    public class ParamDocumentationCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(
            Constants.DiagnosticIdParam,
            Constants.DiagnosticIdProperty);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var name = diagnostic.Properties[Constants.DiagnosticName];
            var description = diagnostic.Properties[Constants.DiagnosticDescription];

            var param = root.FindNode(diagnostic.Location.SourceSpan, true) as XmlElementSyntax;
            var newParam = param.WithContent(
                SyntaxFactory.SingletonList<XmlNodeSyntax>(
                    SyntaxFactory.XmlText().WithTextTokens(
                        SyntaxFactory.TokenList(
                            SyntaxFactory.XmlTextLiteral(SyntaxFactory.TriviaList(), description, description, SyntaxFactory.TriviaList())
                        ))));

            context.RegisterCodeFix(
                CodeAction.Create($"Update '{name}' description",
                    token => Task.FromResult(context.Document.WithSyntaxRoot(root.ReplaceNode(param, newParam))),
                    string.Concat(diagnostic.Id, name)),
                diagnostic);
        }
    }
}
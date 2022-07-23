using System.Composition;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace Karls.Analyzers.Optimizely;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(OptimizelyUpdateContentTypeIdCodeFixProvider)), Shared]
public sealed class OptimizelyUpdateContentTypeIdCodeFixProvider : CodeFixProvider {
    public static Func<Guid> GuidGenerator { get; set; } = () => Guid.NewGuid();

    public override ImmutableArray<string> FixableDiagnosticIds {
        get { return ImmutableArray.Create(DiagnosticIdentifiers.OptimizelyUniqueContentTypeIds); }
    }

    public override FixAllProvider? GetFixAllProvider() {
        return null;
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context) {
        var root = await context.Document.GetSyntaxRootAsync();
        if(root == null)
            return;

        var diagnostic = context.Diagnostics.First();
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        var literalSyntax = root.FindToken(diagnosticSpan.Start);
        if(literalSyntax == default)
            return;

        var reorderPropertiesCodeAction = CodeAction.Create(
            "Generate a new GUID",
            cancellationToken => RefactorAsync(context.Document, literalSyntax, cancellationToken),
            equivalenceKey: "ReorderPropertiesByDisplayOrder");

        context.RegisterCodeFix(reorderPropertiesCodeAction, context.Diagnostics);
    }

    public static async Task<Solution> RefactorAsync(
            Document document,
            SyntaxToken literalSyntax,
            CancellationToken cancellationToken) {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if(root == null) {
            throw new InvalidOperationException("Could not get syntax root");
        }

        var newRoot = root.ReplaceToken(literalSyntax, SyntaxFactory.ParseToken("\"" + GuidGenerator().ToString() + "\""));

        var newDocument = document.WithSyntaxRoot(newRoot);

        return newDocument.Project.Solution;
    }
}

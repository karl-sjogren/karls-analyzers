using System.Composition;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace Karls.Analyzers.BannedStrings;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(BannedStringsCodeFixProvider)), Shared]
public sealed class BannedStringsCodeFixProvider : CodeFixProvider {
    public override ImmutableArray<string> FixableDiagnosticIds {
        get { return [DiagnosticIdentifiers.UsePredefinedConstInsteadOfString]; }
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

        diagnostic.Properties.TryGetValue("Constant", out var replacementConstant);
        if(replacementConstant is null) {
            return;
        }

        var reorderPropertiesCodeAction = CodeAction.Create(
            "Replace with predefined constant",
            cancellationToken => RefactorAsync(context.Document, literalSyntax, replacementConstant, cancellationToken),
            equivalenceKey: "ReorderPropertiesByDisplayOrder");

        context.RegisterCodeFix(reorderPropertiesCodeAction, context.Diagnostics);
    }

    public static async Task<Solution> RefactorAsync(
            Document document,
            SyntaxToken literalSyntax,
            string replacementConstant,
            CancellationToken cancellationToken) {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if(root == null) {
            throw new InvalidOperationException("Could not get syntax root");
        }

        var newRoot = root.ReplaceNode(literalSyntax.Parent!, SyntaxFactory.ParseExpression(replacementConstant));

        var newDocument = document.WithSyntaxRoot(newRoot);

        return newDocument.Project.Solution;
    }
}

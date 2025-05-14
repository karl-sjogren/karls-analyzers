using System.Composition;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace Karls.Analyzers.EmojiStrings;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EmojiStringCodeFixProvider)), Shared]
public class EmojiStringCodeFixProvider : CodeFixProvider {
    public sealed override ImmutableArray<string> FixableDiagnosticIds => [EmojiStringAnalyzer.DiagnosticId];

    public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context) {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        var diagnostic = context.Diagnostics.First();
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        // Find the string literal expression identified by the diagnostic.
        var literalExpr = root!.FindToken(diagnosticSpan.Start).Parent as LiteralExpressionSyntax;
        if(literalExpr == null)
            return;

        context.RegisterCodeFix(
            CodeAction.Create(
                title: "Emoji-encode string",
                createChangedDocument: c => ReplaceWithEmojiEncodedStringAsync(context.Document, literalExpr, c),
                equivalenceKey: "EmojiEncode"),
            diagnostic);
    }

    private async Task<Document> ReplaceWithEmojiEncodedStringAsync(Document document, LiteralExpressionSyntax literalExpr, CancellationToken cancellationToken) {
        var original = literalExpr.Token.ValueText;
        var encoded = EmojiObfuscator.EncodeToEmoji(original);

        // Create the new expression: EmojiDecoder.Decode("encoded")
        var decodeInvocation = SyntaxFactory.ParseExpression($"EmojiDecoder.Decode(\"{encoded}\")")
            .WithTriviaFrom(literalExpr);

        var root = await document.GetSyntaxRootAsync(cancellationToken);
        var newRoot = root?.ReplaceNode(literalExpr, decodeInvocation!);
        return document.WithSyntaxRoot(newRoot!);
    }
}

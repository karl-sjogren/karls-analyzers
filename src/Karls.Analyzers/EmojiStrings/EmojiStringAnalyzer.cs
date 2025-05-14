namespace Karls.Analyzers.EmojiStrings;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class EmojiStringAnalyzer : DiagnosticAnalyzer {
    public const string DiagnosticId = "EMOJI001";
    private static readonly LocalizableString _title = "String literal should be emoji-encoded";
    private static readonly LocalizableString _messageFormat = "String literal '{0}' is not emoji-encoded";
    private const string _category = "Security";

    private static readonly DiagnosticDescriptor _rule = new DiagnosticDescriptor(
        DiagnosticId, _title, _messageFormat, _category,
        DiagnosticSeverity.Warning, isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

    public override void Initialize(AnalysisContext context) {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeStringLiteral, SyntaxKind.StringLiteralExpression);
    }

    private void AnalyzeStringLiteral(SyntaxNodeAnalysisContext context) {
        var literal = (LiteralExpressionSyntax)context.Node;
        string value = literal.Token.ValueText;

        // Skip if the string is already emoji-encoded (simple check)
        if(value.All(c => char.GetUnicodeCategory(c) == System.Globalization.UnicodeCategory.OtherSymbol))
            return;

        var diagnostic = Diagnostic.Create(_rule, literal.GetLocation(), value);
        context.ReportDiagnostic(diagnostic);
    }
}

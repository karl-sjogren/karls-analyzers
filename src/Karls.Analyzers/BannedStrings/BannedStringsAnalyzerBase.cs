using Microsoft.CodeAnalysis.Text;

namespace Karls.Analyzers.BannedStrings;

public abstract class BannedStringsAnalyzerBase : DiagnosticAnalyzer {
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    private readonly SourceTextValueProvider<BannedStringWithReplacement[]> _valueProvider = new(ReadConfigurationFile);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics {
        get {
            if(_supportedDiagnostics.IsDefault)
                ImmutableInterlocked.InterlockedInitialize(ref _supportedDiagnostics, [DiagnosticDescriptor]);

            return _supportedDiagnostics;
        }
    }

    protected abstract DiagnosticDescriptor DiagnosticDescriptor { get; }
    protected abstract string FilePrefix { get; }

    public override void Initialize(AnalysisContext context) {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

        context.RegisterSyntaxNodeAction(f => CheckStringLiteralTokens(f, context), SyntaxKind.StringLiteralExpression, SyntaxKind.InterpolatedStringExpression);
    }

    private void CheckAndReportContent(Action<Diagnostic> reportDiagnostic, BannedStringWithReplacement[] terms, Location location, string nodeText, string content) {
        var matchingTerm = GetMatchingTerms(content, terms);
        if(matchingTerm is null) {
            return;
        }

        reportDiagnostic(Diagnostic.Create(
            descriptor: DiagnosticDescriptor,
            location: location,
            properties: new Dictionary<string, string?> { ["Constant"] = matchingTerm.Replacement }.ToImmutableDictionary(),
            messageArgs: [nodeText, matchingTerm.Replacement]));
    }

    private void CheckStringLiteralTokens(SyntaxNodeAnalysisContext context, AnalysisContext analysisContext) {
        var literalNode = context.Node as LiteralExpressionSyntax;
        var interpolatedNode = context.Node as InterpolatedStringExpressionSyntax;

        if(literalNode is null && interpolatedNode is null) {
            return;
        }

        if(IsConstant(literalNode) || IsConstant(interpolatedNode)) {
            return;
        }

        var terms = GetConfiguredTerms(analysisContext, context.Options.AdditionalFiles);

        if(literalNode is not null) {
            CheckAndReportContent(context.ReportDiagnostic, terms, literalNode.GetLocation(), literalNode.Token.Text, literalNode.Token.ValueText);
        } else if(interpolatedNode is not null) {
            CheckAndReportContent(context.ReportDiagnostic, terms, interpolatedNode.GetLocation(), interpolatedNode.ToFullString(), interpolatedNode.Contents.ToString());
        }
    }

    private static bool IsConstant(ExpressionSyntax? node) {
        if(node is null) {
            return false;
        }

        var fieldDeclaration = node.FirstAncestorOrSelf<FieldDeclarationSyntax>();
        if(fieldDeclaration is null) {
            return false;
        }

        return fieldDeclaration.Modifiers.Any(SyntaxKind.ConstKeyword);
    }

    private static BannedStringWithReplacement? GetMatchingTerms(string content, BannedStringWithReplacement[] terms) {
        return terms.FirstOrDefault(term => content.Equals(term.Term, StringComparison.InvariantCultureIgnoreCase));
    }

    protected BannedStringWithReplacement[] GetConfiguredTerms(AnalysisContext context, ImmutableArray<AdditionalText> additionalFiles) {
        var files = additionalFiles
            .Where(text => Path.GetFileName(text.Path).StartsWith(FilePrefix) && Path.GetFileName(text.Path).EndsWith(".txt"));

        var terms = new List<BannedStringWithReplacement>();
        foreach(var file in files) {
            var text = file.GetText();

            if(text == null)
                continue;

            context.TryGetValue(text, _valueProvider, out var result);
            if(result == null)
                continue;

            terms.AddRange(result);
        }

        if(terms.Count == 0)
            return [];

        return [.. terms];
    }

    private static BannedStringWithReplacement[] ReadConfigurationFile(SourceText text) {
        if(text == null)
            return [];

        var lines = text.Lines;
        if(lines == null)
            return [];

        var result = new List<BannedStringWithReplacement>();
        foreach(var textLine in lines) {
            var line = textLine.ToString();
            var parts = line.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if(parts.Length < 2)
                continue;

            var term = parts[0];
            var replacement = parts[1];
            result.Add(new BannedStringWithReplacement(term, replacement));
        }

        return [.. result];
    }

    protected record BannedStringWithReplacement {
        public BannedStringWithReplacement(string term, string replacement) {
            Term = term;
            Replacement = replacement;
        }

        public string Term { get; init; }
        public string Replacement { get; init; }
    }
}

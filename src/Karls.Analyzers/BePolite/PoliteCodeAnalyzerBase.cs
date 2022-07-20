using Microsoft.CodeAnalysis.Text;

namespace Karls.Analyzers.BePolite;

public abstract class PoliteCodeAnalyzerBase : DiagnosticAnalyzer {
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    private readonly SourceTextValueProvider<TermWithReplacements[]> _valueProvider = new(ReadConfigurationFile);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics {
        get {
            if(_supportedDiagnostics.IsDefault)
                ImmutableInterlocked.InterlockedInitialize(ref _supportedDiagnostics, ImmutableArray.Create(DiagnosticDescriptor));

            return _supportedDiagnostics;
        }
    }

    protected abstract DiagnosticDescriptor DiagnosticDescriptor { get; }
    protected abstract string FilePrefix { get; }
    protected abstract TermWithReplacements[] DefaultTerms { get; }

    public override void Initialize(AnalysisContext context) {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

        context.RegisterSymbolAction(f => AnalyzeSymbol(f, context), SymbolKind.Event, SymbolKind.Field, SymbolKind.Method, SymbolKind.NamedType, SymbolKind.Namespace, SymbolKind.Parameter, SymbolKind.Property);
        context.RegisterSyntaxNodeAction(f => CheckComments(f, context), SyntaxKind.AddAccessorDeclaration, SyntaxKind.CatchDeclaration, SyntaxKind.ClassDeclaration, SyntaxKind.ConstructorDeclaration, SyntaxKind.ConversionOperatorDeclaration, SyntaxKind.DelegateDeclaration, SyntaxKind.DestructorDeclaration, SyntaxKind.EnumDeclaration, SyntaxKind.EnumMemberDeclaration, SyntaxKind.EventDeclaration, SyntaxKind.EventFieldDeclaration, SyntaxKind.FieldDeclaration, SyntaxKind.GetAccessorDeclaration, SyntaxKind.IndexerDeclaration, SyntaxKind.InterfaceDeclaration, SyntaxKind.MethodDeclaration, SyntaxKind.NamespaceDeclaration, SyntaxKind.OperatorDeclaration, SyntaxKind.PropertyDeclaration, SyntaxKind.RemoveAccessorDeclaration, SyntaxKind.SetAccessorDeclaration, SyntaxKind.StructDeclaration, SyntaxKind.UnknownAccessorDeclaration, SyntaxKind.VariableDeclaration);
    }

    private void CheckAndReportContent(Action<Diagnostic> reportDiagnostic, TermWithReplacements[] terms, Location location, string content) {
        var contentContainsTerm = CheckIfContentContainAnyTerm(content, terms);
        if(!contentContainsTerm)
            return;

        reportDiagnostic(Diagnostic.Create(
            descriptor: DiagnosticDescriptor,
            location: location,
            messageArgs: new[] { content }));
    }

    private static bool CheckIfContentContainAnyTerm(string content, TermWithReplacements[] terms) {
        return terms.Any(t => content.IndexOf(t.Term, StringComparison.InvariantCultureIgnoreCase) >= 0);
    }

    private void AnalyzeSymbol(SymbolAnalysisContext context, AnalysisContext analysisContext) {
        var symbol = context.Symbol;
        var name = symbol.Name;

        if(name.StartsWith("get_", StringComparison.OrdinalIgnoreCase) || name.StartsWith("set_", StringComparison.OrdinalIgnoreCase))
            return;

        var terms = GetConfiguredTerms(analysisContext, context.Options.AdditionalFiles);

        CheckAndReportContent(context.ReportDiagnostic, terms, symbol.Locations[0], name);
    }

    private void CheckComments(SyntaxNodeAnalysisContext context, AnalysisContext analysisContext) {
        var node = context.Node;

        var xmlTrivia = node.GetLeadingTrivia()
            .Select(i => i.GetStructure())
            .OfType<DocumentationCommentTriviaSyntax>()
            .FirstOrDefault();

        if(xmlTrivia == null)
            return;

        var terms = GetConfiguredTerms(analysisContext, context.Options.AdditionalFiles);

        CheckAndReportContent(context.ReportDiagnostic, terms, xmlTrivia.GetLocation(), xmlTrivia.ToFullString());
    }

    protected TermWithReplacements[] GetConfiguredTerms(AnalysisContext context, ImmutableArray<AdditionalText> additionalFiles) {
        var files = additionalFiles
            .Where(text => text.Path.StartsWith(FilePrefix) && text.Path.EndsWith(".txt"));

        var terms = new List<TermWithReplacements>();
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
            return DefaultTerms;

        return terms.ToArray();
    }

    private static TermWithReplacements[] ReadConfigurationFile(SourceText text) {
        if(text == null)
            return Array.Empty<TermWithReplacements>();

        var lines = text.Lines;
        if(lines == null)
            return Array.Empty<TermWithReplacements>();

        var result = new List<TermWithReplacements>();
        foreach(var textLine in lines) {
            var line = textLine.ToString();
            var parts = line.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if(parts.Length < 1)
                continue;

            var term = parts[0];
            var allReplacements = parts.Length > 1 ? parts[1] : string.Empty;
            var replacements = allReplacements?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
            result.Add(new TermWithReplacements(term, replacements));
        }

        return result.ToArray();
    }

    protected record TermWithReplacements {
        public TermWithReplacements(string term) {
            Term = term;
            Replacements = Array.Empty<string>();
        }

        public TermWithReplacements(string term, string[] replacements) {
            Term = term;
            Replacements = replacements;
        }

        public string Term { get; init; }
        public string[] Replacements { get; init; }
    }
}

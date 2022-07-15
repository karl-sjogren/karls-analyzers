namespace Karls.Analyzers.BePolite;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class PoliteCodeAnalyzer : PoliteCodeAnalyzerBase {
    protected override DiagnosticDescriptor DiagnosticDescriptor { get; } = DiagnosticRules.PoliteCodeMakesEveryoneHappier;

    protected override string FilePrefix => "PoliteCode";

    protected override TermWithReplacements[] DefaultTerms => new[] {
        new TermWithReplacements("idiot"),
        new TermWithReplacements("asshole"),
        new TermWithReplacements("a-hole"),
        new TermWithReplacements("stupid"),
        new TermWithReplacements("moron"),
        new TermWithReplacements("weirdo")
    };
}

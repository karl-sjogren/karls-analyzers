namespace Karls.Analyzers.BannedStrings;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class BannedStringsAnalyzer : BannedStringsAnalyzerBase {
    protected override DiagnosticDescriptor DiagnosticDescriptor { get; } = DiagnosticRules.UsePredefinedConstInsteadOfString;

    protected override string FilePrefix => "BannedStrings";
}

using Karls.Analyzers.BePolite;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Roslynator.Testing.CSharp;
using Roslynator.Testing.CSharp.Xunit;

namespace Karls.Analyzers.Tests.BePolite;

public class PoliteCodeAnalyzerTests : XunitDiagnosticVerifier<PoliteCodeAnalyzer, NoopCodeFixProvider> {
    public override CSharpTestOptions Options => CSharpTestOptions.Default
        .WithParseOptions(CSharpTestOptions.Default.ParseOptions.WithLanguageVersion(LanguageVersion.CSharp10))
        .WithAllowedCompilerDiagnosticIds(new[] { "CS0414" });

    public DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.PoliteCodeMakesEveryoneHappier;

    [Fact]
    public async Task ShouldReportWordInClassNameAsync() {
        await VerifyDiagnosticAsync(@"
public class [|StupidComponent|] {
}

".ToDiagnosticsData(Descriptor));
    }

    [Fact]
    public async Task ShouldReportWordInPropertyNameAsync() {
        await VerifyDiagnosticAsync(@"
public class NiceComponent {
    public string [|StupidProperty|] { get; set; }
}

".ToDiagnosticsData(Descriptor));
    }

    [Fact]
    public async Task ShouldReportWordInFieldNameAsync() {
        await VerifyDiagnosticAsync(@"
public class NiceComponent {
    private string [|_stupidField|] = ""Nice"";
}

".ToDiagnosticsData(Descriptor));
    }

    [Fact]
    public async Task ShouldReportWordInEnumAsync() {
        await VerifyDiagnosticAsync(@"
public enum [|StupidEnum|] {
    Nice = 1
}

".ToDiagnosticsData(Descriptor));
    }

    [Fact]
    public async Task ShouldReportWordInEnumValueAsync() {
        await VerifyDiagnosticAsync(@"
public enum NiceEnum {
    [|Stupid|] = 1
}

".ToDiagnosticsData(Descriptor));
    }
}

using Karls.Analyzers.BePolite;
using Roslynator.Testing.CSharp;
using Roslynator.Testing.CSharp.Xunit;

using VerifyCS = Karls.Analyzers.Tests.RoslynUtils.CSharpAnalyzerVerifier<Karls.Analyzers.BePolite.PoliteCodeAnalyzer>;

namespace Karls.Analyzers.Tests.BePolite;

public class PoliteCodeAnalyzerTests : XunitDiagnosticVerifier<PoliteCodeAnalyzer, NoopCodeFixProvider> {
    public override CSharpTestOptions Options => CSharpTestOptions.Default
        .WithParseOptions(CSharpTestOptions.Default.ParseOptions.WithLanguageVersion(LanguageVersion.CSharp10))
        .WithAllowedCompilerDiagnosticIds(["CS0414"]);

    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.PoliteCodeMakesEveryoneHappier;

    [Fact]
    public async Task ShouldReportWordInClassNameAsync() {
        await VerifyDiagnosticAsync(@"
public class [|StupidComponent|] {
}

".ToDiagnosticsData());
    }

    [Fact]
    public async Task ShouldReportWordInPropertyNameAsync() {
        await VerifyDiagnosticAsync(@"
public class NiceComponent {
    public string [|StupidProperty|] { get; set; }
}

".ToDiagnosticsData());
    }

    [Fact]
    public async Task ShouldReportWordInFieldNameAsync() {
        await VerifyDiagnosticAsync(@"
public class NiceComponent {
    private string [|_stupidField|] = ""Nice"";
}

".ToDiagnosticsData());
    }

    [Fact]
    public async Task ShouldReportWordInEnumAsync() {
        await VerifyDiagnosticAsync(@"
public enum [|StupidEnum|] {
    Nice = 1
}

".ToDiagnosticsData());
    }

    [Fact]
    public async Task ShouldReportWordInEnumValueAsync() {
        await VerifyDiagnosticAsync(@"
public enum NiceEnum {
    [|Stupid|] = 1
}

".ToDiagnosticsData());
    }

    [Fact]
    public async Task ShouldReportCustomWordFromAdditionalFileAsync() {
        var test = new VerifyCS.Test {
            TestState =
            {
                Sources = { @"
public class {|#0:DonkeyComponent|} {
}

" },
                AdditionalFiles = { ("PoliteCode.txt", "Donkey") }
            }
        };

        test.ExpectedDiagnostics.Add(VerifyCS.Diagnostic(Descriptor.Id).WithLocation(0).WithArguments("DonkeyComponent"));
        await test.RunAsync();
    }

    [Fact]
    public async Task ShouldNotHaveDefaultWordsIfCustomWordsAreLoadedAsync() {
        var test = new VerifyCS.Test {
            TestState =
            {
                Sources = { @"
public class StupidComponent {
}

" },
                AdditionalFiles = { ("PoliteCode.txt", "Donkey") }
            }
        };

        await test.RunAsync();
    }

    [Fact]
    public async Task ShouldReportCustomWordsFromMultipleAdditionalFilesAsync() {
        var test = new VerifyCS.Test {
            TestState =
            {
                Sources = { @"
public class {|#0:DonkeyComponent|} {
}

public class {|#1:KorkadKomponent|} {
}

" },
                AdditionalFiles = { ("PoliteCode.txt", "Donkey"), ("PoliteCode.Swedish.txt", "Korkad") }
            }
        };

        test.ExpectedDiagnostics.Add(VerifyCS.Diagnostic(Descriptor.Id).WithLocation(0).WithArguments("DonkeyComponent"));
        test.ExpectedDiagnostics.Add(VerifyCS.Diagnostic(Descriptor.Id).WithLocation(1).WithArguments("KorkadKomponent"));
        await test.RunAsync();
    }
}

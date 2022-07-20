using Karls.Analyzers.Optimizely;
using Roslynator.Testing.CSharp;

namespace Karls.Analyzers.Tests.Optimizely;

public class OptimizelyUniqueContentTypeIdsTests : OptimizelyAnalyzerTestBase<OptimizelyUniqueContentTypeIdsAnalyzer, NoopCodeFixProvider> {
    public override CSharpTestOptions Options => CSharpTestOptions.Default
        .WithParseOptions(CSharpTestOptions.Default.ParseOptions.WithLanguageVersion(LanguageVersion.CSharp10));

    public DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.OptimizelyUniqueContentTypeIds;

    [Fact]
    public async Task MultipleContentTypesWithSameIdsInSameFileShouldReportAsync() {
        await VerifyDiagnosticAsync(@"
[|[ContentType(GUID = ""00000000-0000-0000-0000-000000000000"")]
public class Block1 {
}|]

[|[ContentType(GUID = ""00000000-0000-0000-0000-000000000000"")]
public class Block2 {
}|]

".ToDiagnosticsData(Descriptor, OptimizelySetupCode));
    }

    [Fact(Skip = "I can't get this working with diagnostics being reported in AdditionalFiles.")]
    public async Task MultipleContentTypesWithSameIdsInDifferentFilesShouldReportAsync() {
        var secondaryCode = @"
[ContentType(DisplayName = ""Block2"", GUID = ""00000000-0000-0000-0000-000000000000"")]
public class Block2 {
}
";
        await VerifyDiagnosticAsync(@"
[|[ContentType(DisplayName = ""Block1"", GUID = ""00000000-0000-0000-0000-000000000000"")]
public class Block1 {
}|]

".ToDiagnosticsData(Descriptor, OptimizelySetupCode, secondaryCode));
    }

    [Fact]
    public async Task MultipleContentTypesWithDifferentIdsInSameFileShouldNotReportAsync() {
        await VerifyNoDiagnosticAsync(@"
[ContentType(GUID = ""00000000-0000-0000-0000-000000000001"")]
public class Block1 {
}

[ContentType(GUID = ""00000000-0000-0000-0000-000000000002"")]
public class Block2 {
}

".ToDiagnosticsData(Descriptor, OptimizelySetupCode));
    }
}

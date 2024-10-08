using Karls.Analyzers.Optimizely;
using Roslynator.Testing.CSharp;

namespace Karls.Analyzers.Tests.Optimizely;

public class OptimizelyUniqueContentTypeIdsAnalyzerTests : OptimizelyAnalyzerTestBase<OptimizelyUniqueContentTypeIdsAnalyzer, OptimizelyUpdateContentTypeIdCodeFixProvider> {
    public override CSharpTestOptions Options => CSharpTestOptions.Default
        .WithParseOptions(CSharpTestOptions.Default.ParseOptions.WithLanguageVersion(LanguageVersion.CSharp10));

    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.OptimizelyUniqueContentTypeIds;

    [Fact]
    public async Task MultipleContentTypesWithSameIdsInSameFileShouldReportAsync() {
        await VerifyDiagnosticAsync(@"
using EPiServer.DataAnnotations;

[ContentType(GUID = [|""00000000-0000-0000-0000-000000000000""|])]
public class Block1 {
}

[ContentType(GUID = [|""00000000-0000-0000-0000-000000000000""|])]
public class Block2 {
}

".ToDiagnosticsData(OptimizelySetupCode));
    }

    [Fact(Skip = "I can't get this working with diagnostics being reported in AdditionalFiles.")]
    public async Task MultipleContentTypesWithSameIdsInDifferentFilesShouldReportAsync() {
        var secondaryCode = @"
using EPiServer.DataAnnotations;

[ContentType(DisplayName = ""Block2"", GUID = ""00000000-0000-0000-0000-000000000000"")]
public class Block2 {
}
";
        await VerifyDiagnosticAsync(@"
using EPiServer.DataAnnotations;

[ContentType(DisplayName = ""Block1"", GUID = [|""00000000-0000-0000-0000-000000000000""|])]
public class Block1 {
}

".ToDiagnosticsData(OptimizelySetupCode, secondaryCode));
    }

    [Fact]
    public async Task MultipleContentTypesWithDifferentIdsInSameFileShouldNotReportAsync() {
        await VerifyNoDiagnosticAsync(@"
using EPiServer.DataAnnotations;

[ContentType(GUID = ""00000000-0000-0000-0000-000000000001"")]
public class Block1 {
}

[ContentType(GUID = ""00000000-0000-0000-0000-000000000002"")]
public class Block2 {
}

".ToDiagnosticsData(OptimizelySetupCode));
    }

    [Fact]
    public async Task MultipleContentTypesWithSameIdsInSameShouldUpdateWithCodeFixAsync() {
        OptimizelyUpdateContentTypeIdCodeFixProvider.GuidGenerator = () => Guid.Parse("00000000-0000-0000-0000-000000000001");
        await VerifyDiagnosticAndFixAsync(@"
using EPiServer.DataAnnotations;

[ContentType(GUID = [|""00000000-0000-0000-0000-000000000000""|])]
public class Block1 {
}

[ContentType(GUID = [|""00000000-0000-0000-0000-000000000000""|])]
public class Block2 {
}

".ToDiagnosticsData(OptimizelySetupCode), @"
using EPiServer.DataAnnotations;

[ContentType(GUID = ""00000000-0000-0000-0000-000000000001"")]
public class Block1 {
}

[ContentType(GUID = ""00000000-0000-0000-0000-000000000000"")]
public class Block2 {
}

".ToExpectedTestState());
    }
}

using Karls.Analyzers.Optimizely;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Roslynator.Testing.CSharp;
using Roslynator.Testing.CSharp.Xunit;

namespace Karls.Analyzers.Tests.Optimizely;

public class OptimizelyUniqueContentTypeIdsTests : XunitDiagnosticVerifier<OptimizelyUniqueContentTypeIds, NoopCodeFixProvider> {
    public override CSharpTestOptions Options => CSharpTestOptions.Default
        .WithParseOptions(CSharpTestOptions.Default.ParseOptions.WithLanguageVersion(LanguageVersion.CSharp10));

    public DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.OptimizelyUniqueContentTypeIds;

    [Fact]
    public async Task MultipleContentTypesWithSameIdsInSameFileShouldReportAsync() {
        await VerifyDiagnosticAsync(@"
using System;
using System.ComponentModel.DataAnnotations;

[AttributeUsage(AttributeTargets.Class)]
public class ContentTypeAttribute : Attribute {
    public string GUID { get; set; }
}

[|[ContentType(GUID = ""00000000-0000-0000-0000-000000000000"")]
public class Block1 {
}|]

[|[ContentType(GUID = ""00000000-0000-0000-0000-000000000000"")]
public class Block2 {
}|]

".ToDiagnosticsData(Descriptor));
    }

    [Fact(Skip = "I can't get this working with diagnostics being reported in AdditionalFiles.")]
    public async Task MultipleContentTypesWithSameIdsInDifferentFilesShouldReportAsync() {
        var secondaryCode = @"
[ContentType(GUID = ""00000000-0000-0000-0000-000000000000"")]
public class Block2 {
}
";
        await VerifyDiagnosticAsync(@"
using System;
using System.ComponentModel.DataAnnotations;

[AttributeUsage(AttributeTargets.Class)]
public class ContentTypeAttribute : Attribute {
    public string GUID { get; set; }
}

[|[ContentType(GUID = ""00000000-0000-0000-0000-000000000000"")]
public class Block1 {
}|]

".ToDiagnosticsData(Descriptor, new[] { secondaryCode }));
    }

    [Fact]
    public async Task MultipleContentTypesWithDifferentIdsInSameFileShouldNotReportAsync() {
        await VerifyNoDiagnosticAsync(@"
using System;
using System.ComponentModel.DataAnnotations;

[AttributeUsage(AttributeTargets.Class)]
public class ContentTypeAttribute : Attribute {
    public string GUID { get; set; }
}

[ContentType(GUID = ""00000000-0000-0000-0000-000000000001"")]
public class Block1 {
}

[ContentType(GUID = ""00000000-0000-0000-0000-000000000002"")]
public class Block2 {
}

".ToDiagnosticsData(Descriptor));
    }
}

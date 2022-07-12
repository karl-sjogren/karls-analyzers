using Karls.Analyzers.Optimizely;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Roslynator.Testing.CSharp;
using Roslynator.Testing.CSharp.Xunit;

namespace Karls.Analyzers.Tests.Optimizely;

public class OptimizelyPropertyOrderAnalyzerTests : XunitDiagnosticVerifier<OptimizelyPropertyOrderAnalyzer, NoopCodeFixProvider> {
    public override CSharpTestOptions Options => CSharpTestOptions.Default
        .WithParseOptions(CSharpTestOptions.Default.ParseOptions.WithLanguageVersion(LanguageVersion.CSharp10));

    public DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.OptimizelyPropertyOrderShouldMatchSourceOrder;

    [Fact]
    public async Task ShouldReportWhenClassIsContentTypeAndPropertiesAreInWrongOrderAsync() {
        await VerifyDiagnosticAsync(@"
using System;
using System.ComponentModel.DataAnnotations;

[AttributeUsage(AttributeTargets.Class)]
public class ContentTypeAttribute : Attribute {
}

[ContentType]
public class Block {
    [|[Display(Order = 2)]
    public virtual string Prop2 { get; set; }|]

    [Display(Order = 1)]
    public virtual string Prop1 { get; set; }
}

".ToDiagnosticsData(Descriptor));
    }

    [Fact]
    public async Task ShouldNotReportWhenClassIsContentTypeAndPropertiesAreInOrderAsync() {
        await VerifyNoDiagnosticAsync(@"
using System;
using System.ComponentModel.DataAnnotations;

[AttributeUsage(AttributeTargets.Class)]
public class ContentTypeAttribute : Attribute {
}

[ContentType]
public class Block {
    [Display(Order = 1)]
    public virtual string Prop1 { get; set; }

    [Display(Order = 2)]
    public virtual string Prop2 { get; set; }
}

".ToDiagnosticsData(Descriptor));
    }

    [Fact]
    public async Task ShouldNotReportWhenClassIsNotContentTypeAndPropertiesAreInWrongOrderAsync() {
        await VerifyNoDiagnosticAsync(@"
using System.ComponentModel.DataAnnotations;

public class Block {
    [Display(Order = 2)]
    public virtual string Prop2 { get; set; }

    [Display(Order = 1)]
    public virtual string Prop1 { get; set; }
}

".ToDiagnosticsData(Descriptor));
    }
}

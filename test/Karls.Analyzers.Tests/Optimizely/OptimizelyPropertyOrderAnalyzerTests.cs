using Karls.Analyzers.Optimizely;
using Roslynator.Testing.CSharp;

namespace Karls.Analyzers.Tests.Optimizely;

public class OptimizelyPropertyOrderAnalyzerTests : OptimizelyAnalyzerTestBase<OptimizelyPropertyOrderAnalyzer, OptimizelyPropertyOrderCodeFixProvider> {
    public override CSharpTestOptions Options => CSharpTestOptions.Default
        .WithParseOptions(CSharpTestOptions.Default.ParseOptions.WithLanguageVersion(LanguageVersion.CSharp10));

    public DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.OptimizelyPropertyOrderShouldMatchSourceOrder;

    [Fact]
    public async Task ShouldReportWhenClassIsContentTypeAndPropertiesAreInWrongOrderAsync() {
        await VerifyDiagnosticAsync(@"
using System.ComponentModel.DataAnnotations;

[ContentType]
public class Block {
    [|[Display(Order = 2)]
    public virtual string Prop2 { get; set; }|]

    [Display(Order = 1)]
    public virtual string Prop1 { get; set; }
}

".ToDiagnosticsData(Descriptor, OptimizelySetupCode));
    }

    [Fact]
    public async Task ShouldNotReportWhenClassIsContentTypeAndPropertiesAreInOrderAsync() {
        await VerifyNoDiagnosticAsync(@"
using System.ComponentModel.DataAnnotations;

[ContentType]
public class Block {
    [Display(Order = 1)]
    public virtual string Prop1 { get; set; }

    [Display(Order = 2)]
    public virtual string Prop2 { get; set; }
}

".ToDiagnosticsData(Descriptor, OptimizelySetupCode));
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

".ToDiagnosticsData(Descriptor, OptimizelySetupCode));
    }

    [Fact]
    public async Task ShouldReorderPropertiesWithCodeFixAsync() {
        await VerifyDiagnosticAndFixAsync(@"
using System.ComponentModel.DataAnnotations;

[ContentType]
public class Block {
    [|[Display(Order = 2)]
    public virtual string Prop2 { get; set; }|]

    [Display(Order = 1)]
    public virtual string Prop1 { get; set; }

    [Display(Order = 3)]
    public virtual string Prop3 { get; set; }
}

".ToDiagnosticsData(Descriptor, OptimizelySetupCode), @"
using System.ComponentModel.DataAnnotations;

[ContentType]
public class Block {
    [Display(Order = 1)]
    public virtual string Prop1 { get; set; }

    [Display(Order = 2)]
    public virtual string Prop2 { get; set; }

    [Display(Order = 3)]
    public virtual string Prop3 { get; set; }
}

".ToExpectedTestState());
    }
}

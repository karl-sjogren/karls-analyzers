using Karls.Analyzers.Optimizely;
using Roslynator.Testing.CSharp;

namespace Karls.Analyzers.Tests.Optimizely;

public class OptimizelyUpdateDisplayOrderCodeFixProviderTests : OptimizelyAnalyzerTestBase<OptimizelyPropertyOrderAnalyzer, OptimizelyUpdateDisplayOrderCodeFixProvider> {
    public override CSharpTestOptions Options => CSharpTestOptions.Default
        .WithParseOptions(CSharpTestOptions.Default.ParseOptions.WithLanguageVersion(LanguageVersion.CSharp10));

    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.OptimizelyPropertyOrderShouldMatchSourceOrder;

    [Fact]
    public async Task ShouldUpdateDisplayOrderForPropertiesWithCodeFixAsync() {
        await VerifyDiagnosticAndFixAsync(@"
using System.ComponentModel.DataAnnotations;
using EPiServer.DataAnnotations;

[ContentType]
public class Block {
    [|[Display(Order = 2)]
    public virtual string Prop1 { get; set; }|]

    [Display(Order = 1)]
    public virtual string Prop2 { get; set; }
}

".ToDiagnosticsData(Descriptor, OptimizelySetupCode), @"
using System.ComponentModel.DataAnnotations;
using EPiServer.DataAnnotations;

[ContentType]
public class Block {
    [Display(Order = 1)]
    public virtual string Prop1 { get; set; }

    [Display(Order = 2)]
    public virtual string Prop2 { get; set; }
}

".ToExpectedTestState());
    }

    [Fact]
    public async Task ShouldUpdateDisplayOrderForPropertiesWithSameOrderWithCodeFixAsync() {
        await VerifyDiagnosticAndFixAsync(@"
using System.ComponentModel.DataAnnotations;
using EPiServer.DataAnnotations;

[ContentType]
public class Block {
    [|[Display(Order = 1)]
    public virtual string Prop1 { get; set; }|]

    [Display(Order = 1)]
    public virtual string Prop2 { get; set; }
}

".ToDiagnosticsData(Descriptor, OptimizelySetupCode), @"
using System.ComponentModel.DataAnnotations;
using EPiServer.DataAnnotations;

[ContentType]
public class Block {
    [Display(Order = 1)]
    public virtual string Prop1 { get; set; }

    [Display(Order = 2)]
    public virtual string Prop2 { get; set; }
}

".ToExpectedTestState());
    }
}

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Roslynator.Testing.CSharp;
using Roslynator.Testing.CSharp.Xunit;

namespace Shorthand.Optimizely.Analyzers.Tests;

public class OptimizelyPropertyOrderAnalyzerTests : XunitDiagnosticVerifier<OptimizelyPropertyOrderAnalyzer, NoopCodeFixProvider> {
    public override CSharpTestOptions Options => CSharpTestOptions.Default
        .WithParseOptions(CSharpTestOptions.Default.ParseOptions.WithLanguageVersion(LanguageVersion.CSharp10));

    public DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.PropertyOrderShouldMatchSourceOrder;

    [Fact]
    public async Task Test1Async() {
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

".ToDiagnosticsData(Descriptor), options: Options.AddConfigOption("dotnet_diagnostic.SO0001.severity", "suggestion"));
    }

    [Fact]
    public async Task Test2Async() {
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

".ToDiagnosticsData(Descriptor), options: Options.AddConfigOption("dotnet_diagnostic.SO0001.severity", "suggestion"));
    }
}

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.Testing.CSharp.Xunit;

namespace Karls.Analyzers.Tests.Optimizely;

public abstract class OptimizelyAnalyzerTestBase<TAnalyzer, TFixProvider> : XunitDiagnosticVerifier<TAnalyzer, TFixProvider>
    where TAnalyzer : DiagnosticAnalyzer, new()
    where TFixProvider : CodeFixProvider, new() {
    public string OptimizelySetupCode { get; } = @"
using System;
using System.ComponentModel.DataAnnotations;

[AttributeUsage(AttributeTargets.Class)]
public class ContentTypeAttribute : Attribute {
    public string GUID { get; set; }
}
";
}

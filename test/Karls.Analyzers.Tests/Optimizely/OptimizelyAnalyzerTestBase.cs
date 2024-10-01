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

namespace EPiServer.DataAnnotations {
    [AttributeUsage(AttributeTargets.Class)]
    public class ContentTypeAttribute : Attribute {
        public string DisplayName { get; set; }
        public string GUID { get; set; }
        public string Description { get; set; }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class AwesomeContentType : ContentTypeAttribute {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class CultureSpecificAttribute : Attribute {
    }
}

public static class SystemTabNames {
    public const string Content = ""Information"";
}

public class BlockData {
}

public class Url {
}
";
}

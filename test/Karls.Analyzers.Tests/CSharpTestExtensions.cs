// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Roslynator.Testing;
using Roslynator.Testing.CSharp;

namespace Karls.Analyzers.Tests;

public static class CSharpTestExtensions {
    public static CSharpTestOptions AddConfigOption(this CSharpTestOptions options, string key, string value) {
        return options.WithConfigOptions(options.ConfigOptions.SetItem(key, value));
    }

    public static CSharpTestOptions AddConfigOption(this CSharpTestOptions options, string key, bool value) {
        return options.AddConfigOption(key, value ? "true" : "false");
    }

    public static CSharpTestOptions EnableConfigOption(this CSharpTestOptions options, string key) {
        return options.AddConfigOption(key, "true");
    }
}

public static class StringExtensions {
    public static DiagnosticTestData ToDiagnosticsData(this string source, DiagnosticDescriptor descriptor) {
        var code = TestCode.Parse(source);

        var data = new DiagnosticTestData(
            descriptor,
            code.Value,
            code.Spans,
            code.AdditionalSpans);

        return data;
    }
}

// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Karls.Analyzers.Tests;

public static class StringExtensions {
    public static DiagnosticTestData ToDiagnosticsData(this string source, DiagnosticDescriptor descriptor) {
        return source.ToDiagnosticsData(descriptor, Array.Empty<AdditionalFile>(), Array.Empty<string>());
    }

    public static DiagnosticTestData ToDiagnosticsData(this string source, DiagnosticDescriptor descriptor, params string[] additionalCode) {
        return source.ToDiagnosticsData(descriptor, Array.Empty<AdditionalFile>(), additionalCode);
    }

    public static DiagnosticTestData ToDiagnosticsData(this string source, DiagnosticDescriptor descriptor, params AdditionalFile[] additionalFiles) {
        return source.ToDiagnosticsData(descriptor, additionalFiles, Array.Empty<string>());
    }

    public static DiagnosticTestData ToDiagnosticsData(this string source, DiagnosticDescriptor descriptor, AdditionalFile[] additionalFiles, string[] additionalCode) {
        var code = TestCode.Parse(source);

        var additionalCodeFiles = additionalCode?.Select(c => new AdditionalFile(c)) ?? Enumerable.Empty<AdditionalFile>();

        var data = new DiagnosticTestData(
            descriptor,
            code.Value,
            code.Spans,
            code.AdditionalSpans,
            additionalCodeFiles.Concat(additionalFiles),
            alwaysVerifyAdditionalLocations: true);

        return data;
    }
}

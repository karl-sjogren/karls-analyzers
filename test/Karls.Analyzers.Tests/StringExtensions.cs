// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Karls.Analyzers.Tests;

public static class StringExtensions {
    public static DiagnosticTestData ToDiagnosticsData(this string source) {
        return source.ToDiagnosticsData([], []);
    }

    public static DiagnosticTestData ToDiagnosticsData(this string source, params string[] additionalCode) {
        return source.ToDiagnosticsData([], additionalCode);
    }

    public static DiagnosticTestData ToDiagnosticsData(this string source, params AdditionalFile[] additionalFiles) {
        return source.ToDiagnosticsData(additionalFiles, []);
    }

    public static DiagnosticTestData ToDiagnosticsData(this string source, AdditionalFile[] additionalFiles, string[] additionalCode) {
        var code = TestCode.Parse(source);

        var additionalCodeFiles = additionalCode?.Select(c => new AdditionalFile(c)) ?? [];

        var data = new DiagnosticTestData(
            code.Value,
            code.Spans,
            code.AdditionalSpans,
            additionalCodeFiles.Concat(additionalFiles),
            alwaysVerifyAdditionalLocations: true);

        return data;
    }

    public static ExpectedTestState ToExpectedTestState(this string source, string codeActionTitle = null) {
        var state = new ExpectedTestState(source, codeActionTitle);

        return state;
    }
}

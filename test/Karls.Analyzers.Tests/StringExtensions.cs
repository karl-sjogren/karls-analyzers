// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Roslynator.Testing;

namespace Karls.Analyzers.Tests;

public static class StringExtensions {
    public static DiagnosticTestData ToDiagnosticsData(this string source, DiagnosticDescriptor descriptor, IEnumerable<string> additionalCode = null) {
        var code = TestCode.Parse(source);

        var additionalFiles = additionalCode?.Select(c => new AdditionalFile(c)) ?? Enumerable.Empty<AdditionalFile>();

        var data = new DiagnosticTestData(
            descriptor,
            code.Value,
            code.Spans,
            code.AdditionalSpans,
            additionalFiles,
            alwaysVerifyAdditionalLocations: true);

        return data;
    }
}

using Microsoft.CodeAnalysis;

namespace Shorthand.Optimizely.Analyzers;

public static class DiagnosticRules {
    public static readonly DiagnosticDescriptor PropertyOrderShouldMatchSourceOrder = new(
            id: DiagnosticIdentifiers.PropertyOrderShouldMatchSourceOrder,
            title: "Properties on content types should have matching sort and source order.",
            messageFormat: "Put properties in correct order.",
            category: "Maintainability",
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: false,
            description: null,
            helpLinkUri: "https://github.com/karl-sjogren/optimizely-analyzers/" + DiagnosticIdentifiers.PropertyOrderShouldMatchSourceOrder, // TODO Make better
            customTags: Array.Empty<string>());
}

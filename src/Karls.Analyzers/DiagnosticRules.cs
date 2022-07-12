using Microsoft.CodeAnalysis;

namespace Karls.Analyzers;

public static class DiagnosticRules {
    public static readonly DiagnosticDescriptor PropertyOrderShouldMatchSourceOrder = new(
            id: DiagnosticIdentifiers.PropertyOrderShouldMatchSourceOrder,
            title: "Properties on content types should have matching sort and source order.",
            messageFormat: "Put properties in correct order.",
            category: "Maintainability",
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: false,
            description: null,
            helpLinkUri: $"https://github.com/karl-sjogren/karls-analyzers/blob/master/docs/analyzers/{DiagnosticIdentifiers.PropertyOrderShouldMatchSourceOrder}.md",
            customTags: Array.Empty<string>());
}

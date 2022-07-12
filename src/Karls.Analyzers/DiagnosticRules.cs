using Microsoft.CodeAnalysis;

namespace Karls.Analyzers;

public static class DiagnosticRules {
    #region Optimizely analyzers

    public static readonly DiagnosticDescriptor OptimizelyPropertyOrderShouldMatchSourceOrder = new(
            id: DiagnosticIdentifiers.OptimizelyPropertyOrderShouldMatchSourceOrder,
            title: "Properties on content types should have matching sort and source order.",
            messageFormat: "Put properties in correct order.",
            category: "Optimizely",
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: false,
            description: null,
            helpLinkUri: GetHelpUrl(DiagnosticIdentifiers.OptimizelyPropertyOrderShouldMatchSourceOrder),
            customTags: Array.Empty<string>());

    public static readonly DiagnosticDescriptor OptimizelyUniqueContentTypeIds = new(
            id: DiagnosticIdentifiers.OptimizelyUniqueContentTypeIds,
            title: "Content types need to have unique identifiers.",
            messageFormat: "Generate a new ID for the content type.",
            category: "Optimizely",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: false,
            description: null,
            helpLinkUri: GetHelpUrl(DiagnosticIdentifiers.OptimizelyUniqueContentTypeIds),
            customTags: Array.Empty<string>());

    #endregion

    private static string GetHelpUrl(string diagnosticId) => $"https://github.com/karl-sjogren/karls-analyzers/blob/master/docs/analyzers/{diagnosticId}.md";
}

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

    #region Be Polite

    public static readonly DiagnosticDescriptor PoliteCodeMakesEveryoneHappier = new(
            id: DiagnosticIdentifiers.PoliteCodeMakesEveryoneHappier,
            title: "Identifiers and comments should not contain impolite or degrading words or sentences.",
            messageFormat: "The word {0} is considered impolite or degrading. Consider renaming/rewriting the affected symbol or comment to be more polite.",
            category: "BePolite",
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: false,
            description: null,
            helpLinkUri: GetHelpUrl(DiagnosticIdentifiers.PoliteCodeMakesEveryoneHappier),
            customTags: Array.Empty<string>());

    public static readonly DiagnosticDescriptor InclusiveCodeMakesEveryoneHappier = new(
            id: DiagnosticIdentifiers.InclusiveCodeMakesEveryoneHappier,
            title: "Identifiers and comments should not contain non-inclusive words or sentences.",
            messageFormat: "The word {0} is not considered inclusive. Consider switching it to a more inclusive word such as {1}.",
            category: "BePolite",
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: false,
            description: null,
            helpLinkUri: GetHelpUrl(DiagnosticIdentifiers.InclusiveCodeMakesEveryoneHappier),
            customTags: Array.Empty<string>());

    #endregion

    private static string GetHelpUrl(string diagnosticId) => $"https://github.com/karl-sjogren/karls-analyzers/blob/main/docs/analyzers/{diagnosticId}.md";
}

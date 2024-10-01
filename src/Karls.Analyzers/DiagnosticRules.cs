namespace Karls.Analyzers;

public static class DiagnosticRules {
    #region Optimizely analyzers

    public static readonly DiagnosticDescriptor OptimizelyPropertyOrderShouldMatchSourceOrder = new(
            id: DiagnosticIdentifiers.OptimizelyPropertyOrderShouldMatchSourceOrder,
            title: "Properties on content types should have matching sort and source order",
            messageFormat: "Put properties in correct order",
            category: "Optimizely",
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: false,
            description: null,
            helpLinkUri: GetHelpUrl(DiagnosticIdentifiers.OptimizelyPropertyOrderShouldMatchSourceOrder),
            customTags: []);

    public static readonly DiagnosticDescriptor OptimizelyUniqueContentTypeIds = new(
            id: DiagnosticIdentifiers.OptimizelyUniqueContentTypeIds,
            title: "Content types need to have unique identifiers",
            messageFormat: "Generate a new ID for the content type",
            category: "Optimizely",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: false,
            description: null,
            helpLinkUri: GetHelpUrl(DiagnosticIdentifiers.OptimizelyUniqueContentTypeIds),
            customTags: []);

    #endregion

    #region Be Polite

    public static readonly DiagnosticDescriptor PoliteCodeMakesEveryoneHappier = new(
            id: DiagnosticIdentifiers.PoliteCodeMakesEveryoneHappier,
            title: "Identifiers and comments should not contain impolite or degrading words or sentences",
            messageFormat: "The word {0} is considered impolite or degrading. Consider renaming/rewriting the affected symbol or comment to be more polite.",
            category: "BePolite",
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: false,
            description: null,
            helpLinkUri: GetHelpUrl(DiagnosticIdentifiers.PoliteCodeMakesEveryoneHappier),
            customTags: []);

    public static readonly DiagnosticDescriptor InclusiveCodeMakesEveryoneHappier = new(
            id: DiagnosticIdentifiers.InclusiveCodeMakesEveryoneHappier,
            title: "Identifiers and comments should not contain non-inclusive words or sentences",
            messageFormat: "The word {0} is not considered inclusive. Consider switching it to a more inclusive word such as {1}.",
            category: "BePolite",
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: false,
            description: null,
            helpLinkUri: GetHelpUrl(DiagnosticIdentifiers.InclusiveCodeMakesEveryoneHappier),
            customTags: []);

    #endregion

    #region Banned strings

    public static readonly DiagnosticDescriptor UsePredefinedConstInsteadOfString = new(
            id: DiagnosticIdentifiers.UsePredefinedConstInsteadOfString,
            title: "Use the predefined constant instead of a string",
            messageFormat: "The literal string {0} should not be used in this project. Use the predefined constant {1} instead.",
            category: "BePolite",
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: false,
            description: null,
            helpLinkUri: GetHelpUrl(DiagnosticIdentifiers.UsePredefinedConstInsteadOfString),
            customTags: []);

    #endregion

    private static string GetHelpUrl(string diagnosticId) => $"https://github.com/karl-sjogren/karls-analyzers/blob/main/docs/analyzers/{diagnosticId}.md";
}

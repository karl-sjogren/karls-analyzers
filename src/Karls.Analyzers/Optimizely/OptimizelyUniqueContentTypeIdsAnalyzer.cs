using System.Collections.Concurrent;

namespace Karls.Analyzers.Optimizely;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class OptimizelyUniqueContentTypeIdsAnalyzer : DiagnosticAnalyzer {
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics {
        get {
            if(_supportedDiagnostics.IsDefault)
                ImmutableInterlocked.InterlockedInitialize(ref _supportedDiagnostics, ImmutableArray.Create(DiagnosticRules.OptimizelyUniqueContentTypeIds));

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context) {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

        context.RegisterCompilationStartAction(compilationAnalysisContext => {
            var identifierLocations = new ConcurrentDictionary<string, List<Location>>();

            compilationAnalysisContext.RegisterSyntaxNodeAction(context => {
                try {
                    AnalyzeClassDeclaration(context, identifierLocations);
                } catch {
                    // :(
                }
            }, SyntaxKind.ClassDeclaration);

            compilationAnalysisContext.RegisterCompilationEndAction(endContext => {
                try {
                    foreach(var identifier in identifierLocations.Where(x => x.Value.Count > 1)) {
                        foreach(var location in identifier.Value) {
                            endContext.ReportDiagnostic(Diagnostic.Create(
                                descriptor: DiagnosticRules.OptimizelyUniqueContentTypeIds,
                                location: location));
                        }
                    }
                } catch {
                    // :(
                }
            });
        });
    }

    private static void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context, ConcurrentDictionary<string, List<Location>> identifierLocations) {
        var node = context.Node as ClassDeclarationSyntax;
        if(node == null)
            return;

        if(!HasContentTypeAttribute(node))
            return;

        var identifier = GetContentTypeId(node);
        if(identifier == null)
            return;

        List<Location>? locations;
        lock(identifierLocations) {
            if(!identifierLocations.TryGetValue(identifier, out locations)) {
                identifierLocations.TryAdd(identifier, new List<Location>() { node.GetLocation() });
                return;
            }
        }

        var currentLocation = node.GetLocation();
        if(locations.Any(x => x == currentLocation))
            return;

        lock(locations) {
            locations.Add(currentLocation);
        }
    }

    private static bool HasContentTypeAttribute(ClassDeclarationSyntax node) {
        return GetContentTypeAttribute(node) != null;
    }

    private static AttributeSyntax? GetContentTypeAttribute(ClassDeclarationSyntax node) {
        if(node == null)
            return null;

        var attributes = node.AttributeLists;
        if(attributes.Count == 0)
            return null;

        var attributeList = attributes.FirstOrDefault(a => a.Attributes.Any(a => a.Name.ToString() == "ContentType"));
        if(attributeList == null)
            return null;

        return attributeList.Attributes.FirstOrDefault(a => a.Name.ToString() == "ContentType");
    }

    private static string? GetContentTypeId(ClassDeclarationSyntax node) {
        var attribute = GetContentTypeAttribute(node);
        if(attribute == null)
            return null;

        var constant = attribute.ArgumentList?.Arguments.Select(x => GetAttributeArgumentValue(x, "GUID")).Where(x => x != null).FirstOrDefault();
        if(constant == null)
            return null;

        return constant.Token.ValueText;
    }

    private static LiteralExpressionSyntax? GetAttributeArgumentValue(AttributeArgumentSyntax node, string argumentName) {
        var argument = node.NameEquals?.Name.ToString();
        if(argument == null)
            return null;

        if(argument != argumentName)
            return null;

        return node.Expression as LiteralExpressionSyntax;
    }
}

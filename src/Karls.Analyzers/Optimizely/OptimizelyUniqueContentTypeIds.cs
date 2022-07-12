using System.Collections.Concurrent;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Karls.Analyzers.Optimizely;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class OptimizelyUniqueContentTypeIds : DiagnosticAnalyzer {
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    private static readonly ConcurrentDictionary<string, List<Location>> _identifierLocations = new();

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

        context.RegisterSyntaxNodeAction(AnalyzeClassDeclaration, SyntaxKind.ClassDeclaration);
    }

    private static void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context) {
        var node = context.Node as ClassDeclarationSyntax;
        if(node == null)
            return;

        if(!HasContentTypeAttribute(node))
            return;

        var identifier = GetContentTypeId(node);
        if(identifier == null)
            return;

        List<Location>? locations;
        lock(_identifierLocations) {
            if(!_identifierLocations.TryGetValue(identifier, out locations)) {
                _identifierLocations.TryAdd(identifier, new List<Location>() { node.GetLocation() });
                return;
            }
        }

        var currentLocation = node.GetLocation();
        if(locations.Any(x => x == currentLocation))
            return;

        lock(locations) {
            locations.Add(currentLocation);
        }

        context.ReportDiagnostic(Diagnostic.Create(
            descriptor: DiagnosticRules.OptimizelyUniqueContentTypeIds,
            location: node.GetLocation()));

        // If this is the second one we find, then add a diagnostic for the first
        // one as well.
        if(locations.Count == 2) {
            context.ReportDiagnostic(Diagnostic.Create(
                descriptor: DiagnosticRules.OptimizelyUniqueContentTypeIds,
                location: locations[0]));
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

        var constant = attribute.ArgumentList?.Arguments.Select(x => GetAttributeArgumentValue(x, "GUID")).FirstOrDefault();
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

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Karls.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class OptimizelyPropertyOrderAnalyzer : DiagnosticAnalyzer {
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics {
        get {
            if(_supportedDiagnostics.IsDefault)
                ImmutableInterlocked.InterlockedInitialize(ref _supportedDiagnostics, ImmutableArray.Create(DiagnosticRules.PropertyOrderShouldMatchSourceOrder));

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context) {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

        context.RegisterSyntaxNodeAction(f => AnalyzeClassDeclaration(f), SyntaxKind.ClassDeclaration);
    }

    private static void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context) {
        var node = context.Node as ClassDeclarationSyntax;
        if(node == null)
            return;

        var attributes = node.AttributeLists;
        if(attributes.Count == 0)
            return;

        if(!attributes.Any(a => a.Attributes.Any(a => a.Name.ToString() == "ContentType")))
            return;

        AnalyzeClassProperties(context, node);
    }

    private static void AnalyzeClassProperties(SyntaxNodeAnalysisContext context, ClassDeclarationSyntax node) {
        var properties = node.Members.OfType<PropertyDeclarationSyntax>();

        var propertyOrders = properties
            .Where(HasDiplayAttribute)
            .Select(x => new PropertyOrder(GetDisplayOrder(x), x))
            .ToArray();

        var unorderedProperty = GetFirstUnorderedProperty(propertyOrders);
        if(unorderedProperty == null)
            return;

        context.ReportDiagnostic(Diagnostic.Create(
            descriptor: DiagnosticRules.PropertyOrderShouldMatchSourceOrder,
            location: unorderedProperty.GetLocation(),
            messageArgs: "Put properties in correct order."));
    }

    private static PropertyDeclarationSyntax? GetFirstUnorderedProperty(PropertyOrder[] propertyOrders) {
        for(var i = 0; i < propertyOrders.Length - 1; i++) {
            if(propertyOrders[i].Order > propertyOrders[i + 1].Order)
                return propertyOrders[i].PropertyDeclaration;
        }

        return null;
    }

    private static bool HasDiplayAttribute(PropertyDeclarationSyntax node) {
        return GetDiplayAttribute(node) != null;
    }

    private static Int32? GetDisplayOrder(PropertyDeclarationSyntax node) {
        var attribute = GetDiplayAttribute(node);
        if(attribute == null)
            return null;

        var constant = attribute.ArgumentList?.Arguments.Select(x => GetAttributeArgumentValue(x, "Order")).FirstOrDefault();
        if(constant == null)
            return null;

        return Int32.Parse(constant.Token.ValueText);
    }

    private static LiteralExpressionSyntax? GetAttributeArgumentValue(AttributeArgumentSyntax node, string argumentName) {
        var argument = node.NameEquals?.Name.ToString();
        if(argument == null)
            return null;

        if(argument != argumentName)
            return null;

        return node.Expression as LiteralExpressionSyntax;
    }

    private static AttributeSyntax? GetDiplayAttribute(PropertyDeclarationSyntax node) {
        if(node == null)
            return null;

        var attributes = node.AttributeLists;
        if(attributes.Count == 0)
            return null;

        var attributeList = attributes.FirstOrDefault(a => a.Attributes.Any(a => a.Name.ToString() == "Display"));
        if(attributeList == null)
            return null;

        return attributeList.Attributes.FirstOrDefault(a => a.Name.ToString() == "Display");
    }

    private record PropertyOrder {
        public PropertyOrder(Int32? order, PropertyDeclarationSyntax propertyDeclaration) {
            Order = order;
            PropertyDeclaration = propertyDeclaration;
        }

        public Int32? Order { get; }
        public PropertyDeclarationSyntax PropertyDeclaration { get; }
    }
}

namespace Karls.Analyzers.Optimizely;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class OptimizelyPropertyOrderAnalyzer : DiagnosticAnalyzer {
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics {
        get {
            if(_supportedDiagnostics.IsDefault)
                ImmutableInterlocked.InterlockedInitialize(ref _supportedDiagnostics, ImmutableArray.Create(DiagnosticRules.OptimizelyPropertyOrderShouldMatchSourceOrder));

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

        var properties = GetOptimizelyPropertiesFromClass(node);

        var (unorderedProperty, isDuplicateOrder) = GetFirstUnorderedProperty(properties);
        if(unorderedProperty == null)
            return;

        context.ReportDiagnostic(Diagnostic.Create(
            descriptor: DiagnosticRules.OptimizelyPropertyOrderShouldMatchSourceOrder,
            location: unorderedProperty.GetLocation(),
            properties: new Dictionary<string, string?> {
                { "IsDuplicateOrder", isDuplicateOrder.ToString() }
            }.ToImmutableDictionary()
        ));
    }

    internal static PropertyWithOrder[] GetOptimizelyPropertiesFromClass(ClassDeclarationSyntax node) {
        var attributes = node.AttributeLists;
        if(attributes.Count == 0)
            return Array.Empty<PropertyWithOrder>();

        if(!attributes.Any(a => a.Attributes.Any(a => a.Name.ToString().EndsWith("ContentType"))))
            return Array.Empty<PropertyWithOrder>();

        return GetOptimizelyPropertiesFromClassProperties(node);
    }

    private static PropertyWithOrder[] GetOptimizelyPropertiesFromClassProperties(ClassDeclarationSyntax node) {
        var properties = node.Members.OfType<PropertyDeclarationSyntax>();

        return properties
            .Where(HasDiplayAttribute)
            .Select(x => new PropertyWithOrder(GetDisplayOrder(x), x))
            .ToArray();
    }

    internal static (PropertyDeclarationSyntax?, bool) GetFirstUnorderedProperty(PropertyWithOrder[] propertyOrders) {
        for(var i = 0; i < propertyOrders.Length - 1; i++) {
            var currentOrder = propertyOrders[i];
            var hasDuplicateOrder = propertyOrders
                .Except(new[] { currentOrder })
                .Any(x => x.Order == currentOrder.Order);

            if(hasDuplicateOrder)
                return (currentOrder.PropertyDeclaration, true);
        }

        for(var i = 0; i < propertyOrders.Length - 1; i++) {
            if(propertyOrders[i].Order > propertyOrders[i + 1].Order)
                return (propertyOrders[i].PropertyDeclaration, false);
        }

        return (null, false);
    }

    private static bool HasDiplayAttribute(PropertyDeclarationSyntax node) {
        return GetDiplayAttribute(node) != null;
    }

    private static int? GetDisplayOrder(PropertyDeclarationSyntax node) {
        var attribute = GetDiplayAttribute(node);
        if(attribute == null)
            return null;

        var constant = attribute.ArgumentList?.Arguments.Select(x => GetAttributeArgumentValue(x, "Order")).Where(x => x != null).FirstOrDefault();
        if(constant == null)
            return null;

        return int.Parse(constant.Token.ValueText);
    }

    private static LiteralExpressionSyntax? GetAttributeArgumentValue(AttributeArgumentSyntax node, string argumentName) {
        var argument = node.NameEquals?.Name.ToString();
        if(argument == null)
            return null;

        if(argument != argumentName)
            return null;

        return node.Expression as LiteralExpressionSyntax;
    }

    internal static AttributeSyntax? GetDiplayAttribute(PropertyDeclarationSyntax node) {
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

    internal record PropertyWithOrder {
        public PropertyWithOrder(int? order, PropertyDeclarationSyntax propertyDeclaration) {
            Order = order;
            PropertyDeclaration = propertyDeclaration;
        }

        public int? Order { get; }
        public PropertyDeclarationSyntax PropertyDeclaration { get; }
    }
}

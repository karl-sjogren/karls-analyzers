using System.Composition;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace Karls.Analyzers.Optimizely;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(OptimizelyUpdateDisplayOrderCodeFixProvider)), Shared]
public sealed class OptimizelyUpdateDisplayOrderCodeFixProvider : CodeFixProvider {
    public override ImmutableArray<string> FixableDiagnosticIds {
        get { return ImmutableArray.Create(DiagnosticIdentifiers.OptimizelyPropertyOrderShouldMatchSourceOrder); }
    }

    public override FixAllProvider? GetFixAllProvider() {
        return null;
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context) {
        var root = await context.Document.GetSyntaxRootAsync();
        if(root == null)
            return;

        var diagnostic = context.Diagnostics.First();
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        var declaration = root.FindToken(diagnosticSpan.Start).Parent?.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().First();
        if(declaration == null)
            return;

        var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>();

        var updateDisplayOrderCodeAction = CodeAction.Create(
            "Update display order to match property order",
            cancellationToken => RefactorAsync(context.Document, declaration, cancellationToken), // TODO Implement another refactoring for this
            equivalenceKey: "UpdateDisplayOrderToMatchPropertyOrder");

        context.RegisterCodeFix(updateDisplayOrderCodeAction, context.Diagnostics);
    }

    public static async Task<Solution> RefactorAsync(
            Document document,
            ClassDeclarationSyntax classDeclaration,
            CancellationToken cancellationToken) {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if(root == null) {
            throw new InvalidOperationException("Could not get syntax root");
        }

        var properties = OptimizelyPropertyOrderAnalyzer.GetOptimizelyPropertiesFromClass(classDeclaration);
        var numberOfProperties = properties.Length;

        var startingDisplayOrder = properties.Select(x => x.Order).Min() ?? 1;

        var newClassDeclaration = classDeclaration;
        for(var i = 0; i < numberOfProperties; i++) {
            var currentProperty = OptimizelyPropertyOrderAnalyzer.GetOptimizelyPropertiesFromClass(newClassDeclaration).Skip(i).First();
            var newProperty = UpdatePropertyDisplayOrder(currentProperty.PropertyDeclaration, startingDisplayOrder + i);

            newClassDeclaration = newClassDeclaration.ReplaceNode(currentProperty.PropertyDeclaration, newProperty);
        }

        var newRoot = root!.ReplaceNode(classDeclaration, newClassDeclaration!);

        var newDocument = document.WithSyntaxRoot(newRoot);

        return newDocument.Project.Solution;
    }

    public static PropertyDeclarationSyntax UpdatePropertyDisplayOrder(PropertyDeclarationSyntax propertyDeclaration, Int32 newOrder) {
        var displayAttribute = OptimizelyPropertyOrderAnalyzer.GetDiplayAttribute(propertyDeclaration);
        if(displayAttribute == null)
            return propertyDeclaration;

        var attributeArgument = displayAttribute.ArgumentList?.Arguments.Where(x => x.NameEquals?.Name.ToString() == "Order").FirstOrDefault();
        if(attributeArgument == null)
            return propertyDeclaration;

        var newAttributeArgument = attributeArgument.WithExpression(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(newOrder)));
        return propertyDeclaration.ReplaceNode(attributeArgument, newAttributeArgument);
    }
}

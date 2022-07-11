using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CodeFixes;

namespace Shorthand.Optimizely.Analyzers.Tests;

public class NoopCodeFixProvider : CodeFixProvider {
    public override ImmutableArray<string> FixableDiagnosticIds => throw new NotSupportedException();

    public override Task RegisterCodeFixesAsync(CodeFixContext context) => throw new NotSupportedException();

    public override FixAllProvider GetFixAllProvider() => throw new NotSupportedException();
}

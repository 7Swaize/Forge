using Microsoft.CodeAnalysis;

namespace Forge.Analyzers.Features.NoAllocAnalyzers.Diagnostics;

internal static class ClosureAllocationDiagnostic {
    internal static DiagnosticDescriptor CreateDescriptor() =>
        new DiagnosticDescriptor(
            id: "FRG0401",
            title: "Heap allocation: Delegate creation",
            messageFormat: "Delegate allocation: new `{0}` instance creation.",
            category: "Usage",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: null,
            helpLinkUri: null
        );

    internal static Diagnostic CreateDiagnostic(Location location, ITypeSymbol delegateInstanceType) =>
        Diagnostic.Create(
            CreateDescriptor(),
            location,
            delegateInstanceType.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)
        );
}
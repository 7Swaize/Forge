using Microsoft.CodeAnalysis;

namespace Forge.Analyzers.Features.NoAllocAnalyzers.Diagnostics;

internal static class HeapAllocationObjectCreationDiagnostic {
    internal static DiagnosticDescriptor CreateDescriptor() =>
        new DiagnosticDescriptor(
            id: "FRG0403",
            title: "Heap allocation: Object creation",
            messageFormat: "Explicit allocation: creation of reference type '{0}' allocates on heap",
            category: "Usage",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: null,
            helpLinkUri: null
        );

    internal static Diagnostic CreateDiagnostic(Location location, ISymbol targetType) =>
        Diagnostic.Create(
            CreateDescriptor(),
            location,
            targetType.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)
        );
}
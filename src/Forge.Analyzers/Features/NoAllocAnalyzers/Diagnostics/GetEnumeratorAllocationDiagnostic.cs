using Microsoft.CodeAnalysis;

namespace Forge.Analyzers.Features.NoAllocAnalyzers.Diagnostics;

internal static class GetEnumeratorAllocationDiagnostic {
    internal static DiagnosticDescriptor CreateDescriptor() =>
        new DiagnosticDescriptor(
            id: "FRG0405",
            title: "Heap allocation: Enumerator allocation",
            messageFormat: "Enumerator allocation: 'foreach' loop over '{0}' allocates reference enumerator on heap",
            category: "Usage",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: null,
            helpLinkUri: null
        );

    internal static Diagnostic CreateDiagnostic(Location location, ISymbol concept) =>
        Diagnostic.Create(
            CreateDescriptor(),
            location,
            concept.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)
        );
}
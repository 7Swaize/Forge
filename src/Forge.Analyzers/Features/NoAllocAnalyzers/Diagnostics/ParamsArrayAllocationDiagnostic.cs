using Microsoft.CodeAnalysis;

namespace Forge.Analyzers.Features.NoAllocAnalyzers.Diagnostics;

internal static class ParamsArrayAllocationDiagnostic {
    internal static DiagnosticDescriptor CreateDescriptor() =>
        new DiagnosticDescriptor(
            id: "FRG0402",
            title: "Creation of params array causes heap allocation",
            messageFormat: "Marked concept {0} forbids heap allocations",
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
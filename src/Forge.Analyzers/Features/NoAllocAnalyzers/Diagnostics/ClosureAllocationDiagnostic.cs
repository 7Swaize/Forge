using Microsoft.CodeAnalysis;

namespace Forge.Analyzers.Features.NoAllocAnalyzers.Diagnostics;

internal static class ClosureAllocationDiagnostic {
    internal static DiagnosticDescriptor CreateDescriptor() =>
        new DiagnosticDescriptor(
            id: "FRG0401",
            title: "Capturing delegate causes heap allocation",
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
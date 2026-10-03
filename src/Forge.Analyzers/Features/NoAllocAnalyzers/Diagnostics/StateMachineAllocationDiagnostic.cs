using Microsoft.CodeAnalysis;

namespace Forge.Analyzers.Features.NoAllocAnalyzers.Diagnostics;

internal static class StateMachineAllocationDiagnostic {
    internal static DiagnosticDescriptor CreateDescriptor() =>
        new DiagnosticDescriptor(
            id: "FRG0404",
            title: "Heap allocation: State machine creation",
            messageFormat: "State machine allocation: '{0}' creates an async or iterator state machine",
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
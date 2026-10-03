using Microsoft.CodeAnalysis;

namespace Forge.Analyzers.Features.NoAllocAnalyzers.Diagnostics;

internal static class ParamsArrayAllocationDiagnostic {
    internal static DiagnosticDescriptor CreateDescriptor() =>
        new DiagnosticDescriptor(
            id: "FRG0402",
            title: "Heap allocation: Implicit params array creation",
            messageFormat: "Implicit allocation: call to method with 'params' parameter allocates '{0}' array",
            category: "Usage",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: null,
            helpLinkUri: null
        );
    
    internal static Diagnostic CreateDiagnostic(Location location, ITypeSymbol arrayType) =>
        Diagnostic.Create(
            CreateDescriptor(),
            location,
            arrayType.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)
        );
}
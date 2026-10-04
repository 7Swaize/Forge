using Microsoft.CodeAnalysis;

namespace Forge.Analyzers.Features.BlittableAnalyzers.Diagnostics;

internal static class ContainedTypeNotBlittableDiagnostic {
    internal static DiagnosticDescriptor CreateDescriptor() =>
        new DiagnosticDescriptor(
            id: "FRG0600",
            title: "Blittable structure does not contain blittable types",
            messageFormat: "The type '{0}' is not blittable",
            category: "MemoryLayout",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: null,
            helpLinkUri: null
        );
    
    internal static Diagnostic CreateDiagnostic(Location location, ITypeSymbol targetType) =>
        Diagnostic.Create(
            CreateDescriptor(),
            location,
            targetType.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)
        );
}
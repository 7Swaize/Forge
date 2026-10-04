using Microsoft.CodeAnalysis;

namespace Forge.Analyzers.Features.BlittableAnalyzers.Diagnostics;

internal static class IncompatibleLayoutDiagnostic {
    internal static DiagnosticDescriptor CreateDescriptor() =>
        new DiagnosticDescriptor(
            id: "FRG0601",
            title: "Incompatible StructLayout for Blittable Type",
            messageFormat: "Struct '{0}' is marked as [Blittable] but uses LayoutKind.Auto",
            category: "MemoryLayout",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: null,
            helpLinkUri: null
        );
    
    internal static Diagnostic CreateDiagnostic(Location location, INamedTypeSymbol targetStruct) =>
        Diagnostic.Create(
            CreateDescriptor(),
            location,
            targetStruct.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)
        );
}
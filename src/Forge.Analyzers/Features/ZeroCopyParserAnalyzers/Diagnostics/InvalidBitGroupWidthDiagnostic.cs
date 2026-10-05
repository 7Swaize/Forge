using Microsoft.CodeAnalysis;

namespace Forge.Analyzers.Features.ZeroCopyParserAnalyzers.Diagnostics;

internal static class InvalidBitGroupWidthDiagnostic {
    internal static DiagnosticDescriptor CreateDescriptor() =>
        new DiagnosticDescriptor(
            id: "FRG0601",
            title: "Bit group width must total exactly 8, 16, 32 or 64 bits",
            messageFormat: "Bit group width of '{0}' is invalid",
            category: "Usage",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: null,
            helpLinkUri: null
        );
    
    internal static Diagnostic CreateDiagnostic(Location location, int bitWidth) =>
        Diagnostic.Create(
            CreateDescriptor(),
            location,
            bitWidth
        );
}
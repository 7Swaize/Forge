using Microsoft.CodeAnalysis;

namespace Forge.Analyzers.Features.ZeroCopyParserAnalyzers.Diagnostics;

internal static class LengthFromAttributeInvalidInputDiagnostic {
    internal static DiagnosticDescriptor CreateDescriptor() =>
        new DiagnosticDescriptor(
            id: "FRG0606",
            title: "[LengthFrom] source must be an unscaled integer of at most 32 bits",
            messageFormat: "Field '{0}' does not meet this criteria",
            category: "Usage",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: null,
            helpLinkUri: null
        );
    
    internal static Diagnostic CreateDiagnostic(Location location, IFieldSymbol field) =>
        Diagnostic.Create(
            CreateDescriptor(),
            location,
            field.Name
        );
}
using Microsoft.CodeAnalysis;

namespace Forge.Analyzers.Features.RemoveByAnalyzers.Diagnostics;

internal static class PostRemoveByDataDiagnostic {
    internal static DiagnosticDescriptor CreateDescriptor() =>
        new DiagnosticDescriptor(
            id: "FRG0501",
            title: "Code Removal Date Passed",
            messageFormat: "The removal date ({0}) for this code has passed. It must be removed or refactored immediately",
            category: "Maintainability",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Code marked with the [RemoveBy] attribute has passed its expiration date and now generates an error to force cleanup."
        );
    
    internal static Diagnostic CreateDiagnostic(Location location, string date) =>
        Diagnostic.Create(
            CreateDescriptor(),
            location,
            date
        );
}
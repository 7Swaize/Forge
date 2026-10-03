using Microsoft.CodeAnalysis;

namespace Forge.Analyzers.Features.RemoveByAnalyzers.Diagnostics;

internal static class PreRemoveByDateDiagnostic {
    internal static DiagnosticDescriptor CreateDescriptor() =>
        new DiagnosticDescriptor(
            id: "FRG0500",
            title: "Code Scheduled for Removal",
            messageFormat: "This code is scheduled for removal on {0}. Please plan to refactor or remove this temporary hack",
            category: "Maintainability",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Code marked with the [RemoveBy] attribute should be cleaned up before the specified date."
        );
    
    internal static Diagnostic CreateDiagnostic(Location location, string date) =>
        Diagnostic.Create(
            CreateDescriptor(),
            location,
            date
        );
}

using Microsoft.CodeAnalysis;

namespace Forge.Analyzers.Features.RemoveByAnalyzers.Diagnostics;

internal static class InvalidDateTimeFormatDiagnostic {
    internal static DiagnosticDescriptor CreateDescriptor() =>
        new DiagnosticDescriptor(
            id: "FRG0502",
            title: "Invalid Date Format",
            messageFormat: "The provided date string '{0}' is not a valid date format. Expected format is 'yyyy-MM-dd'",
            category: "Usage",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Dates provided to the [RemoveBy] attribute must be valid and correctly formatted to be evaluated.",
            helpLinkUri: null
        );
    
    internal static Diagnostic CreateDiagnostic(Location location, string invalidDate) =>
        Diagnostic.Create(
            CreateDescriptor(),
            location,
            invalidDate
        );
}
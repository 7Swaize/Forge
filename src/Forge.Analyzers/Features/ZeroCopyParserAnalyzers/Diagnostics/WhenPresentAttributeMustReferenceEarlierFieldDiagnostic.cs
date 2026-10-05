using Microsoft.CodeAnalysis;

namespace Forge.Analyzers.Features.ZeroCopyParserAnalyzers.Diagnostics;

internal static class WhenPresentAttributeMustReferenceEarlierFieldDiagnostic {
    internal static DiagnosticDescriptor CreateDescriptor() =>
        new DiagnosticDescriptor(
            id: "FRG0604",
            title: "[WhenPresent] attribute must name an earlier bool field",
            messageFormat: "Field '{0}' is not an earlier bool field",
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
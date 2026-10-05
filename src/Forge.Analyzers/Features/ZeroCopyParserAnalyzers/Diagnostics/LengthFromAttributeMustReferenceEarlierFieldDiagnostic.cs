using Microsoft.CodeAnalysis;

namespace Forge.Analyzers.Features.ZeroCopyParserAnalyzers.Diagnostics;

public class LengthFromAttributeMustReferenceEarlierFieldDiagnostic {
    internal static DiagnosticDescriptor CreateDescriptor() =>
        new DiagnosticDescriptor(
            id: "FRG0605",
            title: "[LengthFrom] attribute must name an earlier int field",
            messageFormat: "Field '{0}' is not an earlier int field",
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
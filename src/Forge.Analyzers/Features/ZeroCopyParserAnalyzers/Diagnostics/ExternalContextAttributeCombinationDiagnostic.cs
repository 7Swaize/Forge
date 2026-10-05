using System.Linq;
using Microsoft.CodeAnalysis;

namespace Forge.Analyzers.Features.ZeroCopyParserAnalyzers.Diagnostics;

internal static class ExternalContextAttributeCombinationDiagnostic {
    internal static DiagnosticDescriptor CreateDescriptor() =>
        new DiagnosticDescriptor(
            id: "FRG0600",
            title: "Attribute [ExternalContext] cannot be combined with other modifiers",
            messageFormat: "Cannot combine '[ExternalContext]' with modifiers: '{0}'",
            category: "Usage",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: null,
            helpLinkUri: null
        );
    
        
    internal static Diagnostic CreateDiagnostic(Location location, params ITypeSymbol[] attrs) =>
        Diagnostic.Create(
            CreateDescriptor(),
            location,
            string.Join(", ", attrs.Select(attr => attr.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)))
        );
}
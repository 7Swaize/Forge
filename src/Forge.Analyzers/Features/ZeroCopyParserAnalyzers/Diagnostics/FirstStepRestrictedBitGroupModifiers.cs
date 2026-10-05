using System.Linq;
using Microsoft.CodeAnalysis;

namespace Forge.Analyzers.Features.ZeroCopyParserAnalyzers.Diagnostics;

internal static class FirstStepRestrictedBitGroupModifiers {
    internal static DiagnosticDescriptor CreateDescriptor() =>
        new DiagnosticDescriptor(
            id: "FRG0603",
            title: "Select modifiers are only allowed on the first field of a bit group",
            messageFormat: "Currently applied modifiers are not valid: '{0}'",
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
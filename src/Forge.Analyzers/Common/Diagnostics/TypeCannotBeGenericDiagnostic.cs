using Microsoft.CodeAnalysis;

namespace Forge.Analyzers.Common.Diagnostics;

internal static class TypeCannotBeGenericDiagnostic {
    internal static DiagnosticDescriptor CreateDescriptor() =>
        new DiagnosticDescriptor(
            id: "FRG0002",
            title: "Type cannot be generic",
            messageFormat: "The type '{0}' that the attribute is applied to cannot be generic",
            category: "Usage",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Attributes of this type cannot be applied to generic types.",
            helpLinkUri: null
        );
    
    internal static Diagnostic CreateDiagnostic(Location location, ITypeSymbol type) =>
        Diagnostic.Create(
            CreateDescriptor(),
            location,
            type.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)
        );
}
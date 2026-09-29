using Microsoft.CodeAnalysis;

namespace Forge.Analyzers.Features.CompileTimeConstantAttribute.Diagnostics;

internal static class AttributeOnInvalidTargetTypeDiagnostic {
    internal static DiagnosticDescriptor CreateDescriptor() =>
        new DiagnosticDescriptor(
            id: "FRG0301",
            title: "Attribute applied to a type that cannot be a compile-time constant",
            messageFormat: "The type '{0}' can never be a compile-time constant",
            category: "Usage",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "No value of the type is considered a compile-time constant.",
            helpLinkUri: null
        );
    
    internal static Diagnostic CreateDiagnostic(Location location, ITypeSymbol type) =>
        Diagnostic.Create(
            CreateDescriptor(),
            location,
            type.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)
        );
}
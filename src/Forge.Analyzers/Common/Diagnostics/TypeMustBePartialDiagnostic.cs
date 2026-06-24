using Microsoft.CodeAnalysis;

namespace Forge.Analyzers.Common.Diagnostics;

internal static class TypeMustBePartialDiagnostic {
    internal static DiagnosticDescriptor CreateDescriptor() =>
        new DiagnosticDescriptor(
            id: "FRG0001",
            title: "Type must be partial",
            messageFormat: "Type '{0}' must be declared as partial",
            category: "Usage",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description:
                "The target type must be declared with the 'partial' modifier in order to support source generation.",
            helpLinkUri: null
        );

    internal static Diagnostic CreateDiagnostic(Location location, ITypeSymbol type) =>
        Diagnostic.Create(
            CreateDescriptor(),
            location,
            type.GetGenericArityFQN(includeGlobal: false)
        );
}
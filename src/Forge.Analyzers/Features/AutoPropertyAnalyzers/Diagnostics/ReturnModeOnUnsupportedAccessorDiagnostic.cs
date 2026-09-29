using Forge.Annotations;
using Microsoft.CodeAnalysis;

namespace Forge.Analyzers.Features.AutoPropertyAnalyzer.Diagnostics;

internal static class ReturnModeOnUnsupportedAccessorDiagnostic {
    internal static DiagnosticDescriptor CreateDescriptor() =>
        new DiagnosticDescriptor(
            id: "FRG0100",
            title: "Return mode on unsupported Accessor permutation",
            messageFormat:
                "Return mode '{0}' is unsupported on accessor permutation '{1}",
            category: "Usage",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description:
                "The specified ReturnMode is not supported for the chosen combination of Accessors. " +
                "Ensure the selected return behavior is compatible with the configured property accessors.",
            helpLinkUri: null
        );

    internal static Diagnostic CreateDiagnostic(Location location, ReturnMode returnMode, Accessors accessor) =>
        Diagnostic.Create(
            CreateDescriptor(),
            location,
            returnMode,
            accessor
        );
}
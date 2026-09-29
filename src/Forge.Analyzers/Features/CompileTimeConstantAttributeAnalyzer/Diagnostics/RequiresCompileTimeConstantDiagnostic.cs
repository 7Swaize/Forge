using Microsoft.CodeAnalysis;

namespace Forge.Analyzers.Features.CompileTimeConstantAttribute.Diagnostics;

internal static class RequiresCompileTimeConstantDiagnostic {
    internal static DiagnosticDescriptor CreateDescriptor() =>
        new DiagnosticDescriptor(
            id: "FRG0300",
            title: "Argument must be a compile-time constant",
            messageFormat: "The argument passed to parameter '{0}' must be a compile-time constant",
            category: "Usage",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Parameters decorated with [CompileTimeConstant] strictly require constants or literals.",
            helpLinkUri: null
        );

    internal static Diagnostic CreateDiagnostic(Location location, string parameterName) =>
        Diagnostic.Create(
            CreateDescriptor(),
            location,
            parameterName
        );
}
using Forge.Annotations;
using Microsoft.CodeAnalysis;

namespace Forge.Analyzers.Features.NoLocalsAnalyzer.Diagnostics;

internal static class MethodCannotContainLocalVariableDeclarationsDiagnostic {
    internal static DiagnosticDescriptor CreateDescriptor() =>
        new DiagnosticDescriptor(
            id: "FRG0200",
            title: "Method or Anonymous Function cannot contain local variable declarations.",
            messageFormat:
                $"Local variable declaration is disallowed within method annotated with {typeof(NoLocalVariablesAttribute).FullName}",
            category: "Usage",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description:
                "Method or Anonymous Function cannot contain local variable declarations.",
            helpLinkUri: null
        );

    internal static Diagnostic CreateDiagnostic(Location location) =>
        Diagnostic.Create(
            CreateDescriptor(),
            location
        );
}
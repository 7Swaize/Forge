using System.Linq;
using Forge.Analyzers.Common.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Forge.Analyzers.Common.Discovery;

internal static class CommonDiagnosticParser {
    internal static Diagnostic? CheckContainingTypeMustBePartialDiagnostic(ISymbol symbol) {
        INamedTypeSymbol containingType = symbol.ContainingType;

        bool isPartial = containingType.DeclaringSyntaxReferences.Any(syntax =>
            syntax.GetSyntax() is BaseTypeDeclarationSyntax declaration
            && declaration.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PartialKeyword))
        );

        return !isPartial
            ? TypeMustBePartialDiagnostic.CreateDiagnostic(containingType.Locations[0], containingType)
            : null;
    }
}
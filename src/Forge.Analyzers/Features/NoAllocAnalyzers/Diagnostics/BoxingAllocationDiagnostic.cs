using Microsoft.CodeAnalysis;

namespace Forge.Analyzers.Features.NoAllocAnalyzers.Diagnostics;

internal static class BoxingAllocationDiagnostic {
    internal static DiagnosticDescriptor CreateDescriptor() =>
        new DiagnosticDescriptor(
            id: "FRG0400",
            title: "Heap allocation: Boxing conversion",
            messageFormat: "Boxing allocation: conversion of '{0}' to '{1}' allocates on heap",
            category: "Usage",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "This type of construct forbids heap allocations.",
            helpLinkUri: null
        );

    internal static Diagnostic CreateDiagnostic(Location location, ITypeSymbol fromType, ITypeSymbol toType) =>
        Diagnostic.Create(
            CreateDescriptor(),
            location,
            fromType.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat),
            toType.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)
        );
}
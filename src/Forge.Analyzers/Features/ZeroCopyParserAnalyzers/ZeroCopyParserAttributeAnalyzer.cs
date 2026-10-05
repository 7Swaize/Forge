using System.Collections.Immutable;
using Forge.Analyzers.Common.Diagnostics;
using Forge.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Forge.Analyzers.Features.ZeroCopyParserAnalyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal sealed class ZeroCopyParserAttributeAnalyzer : DiagnosticAnalyzer {
    public override void Initialize(AnalysisContext context) {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(static compilationContext => {
            INamedTypeSymbol? targetAttr =
                compilationContext.Compilation.GetTypeByMetadataName(typeof(ZeroCopyParserAttribute).FullName!);

            if (targetAttr == null) {
                return;
            }

            compilationContext.RegisterSymbolAction(ctx => AnalyzeNamedType(ref ctx, targetAttr), SymbolKind.NamedType);
        });
    }

    private static void AnalyzeNamedType(ref SymbolAnalysisContext ctx, INamedTypeSymbol targetAttr) {
        if (ctx.Symbol is not INamedTypeSymbol target || !target.ValidateAnnotatedWith(targetAttr)) {
            return;
        }
        
        ReportIfNotPartial(ref ctx, target);
        ReportIfUnboundGeneric(ref ctx, target);
    }

    private static void ReportIfNotPartial(ref SymbolAnalysisContext ctx, INamedTypeSymbol target) {
        if (!target.IsPartialDeclaration()) {
            ctx.ReportDiagnostic(TypeMustBePartialDiagnostic.CreateDiagnostic(
                target.Locations[0],
                target
            ));
        }
    }

    private static void ReportIfUnboundGeneric(ref SymbolAnalysisContext ctx, INamedTypeSymbol target) {
        if (target.IsUnboundGenericType) {
            ctx.ReportDiagnostic(TypeCannotBeGenericDiagnostic.CreateDiagnostic(
                target.Locations[0],
                target
            ));
        }
    }

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
        TypeMustBePartialDiagnostic.CreateDescriptor(),
        TypeCannotBeGenericDiagnostic.CreateDescriptor()
    );
}
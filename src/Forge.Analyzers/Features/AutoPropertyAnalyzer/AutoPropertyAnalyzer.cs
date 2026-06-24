using System.Collections.Immutable;
using Forge.Analyzers.Common.Diagnostics;
using Forge.Analyzers.Features.AutoPropertyAnalyzer.Diagnostics;
using Forge.Analyzers.Features.AutoPropertyAnalyzer.Discovery;
using Forge.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Forge.Analyzers.Features.AutoPropertyAnalyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AutoPropertyAnalyzer : DiagnosticAnalyzer{
    public override void Initialize(AnalysisContext context) {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(static compilationContext => {
            INamedTypeSymbol? targetAttr =
                compilationContext.Compilation.GetTypeByMetadataName(typeof(AutoPropertyAttribute).FullName!);

            if (targetAttr == null) {
                return;
            }

            compilationContext.RegisterSymbolAction(ctx => AnalyzeSymbol(ctx, targetAttr), SymbolKind.Field);
        });
    }

    private static void AnalyzeSymbol(SymbolAnalysisContext context, INamedTypeSymbol targetAttr) {
        if (context.Symbol is not IFieldSymbol field) {
            return;
        }

        if (!field.ValidateAnnotatedWith(targetAttr, out AttributeData? attribute)) {
            return;
        }

        ParseAutoPropertyAttribute.Parse(field, attribute).ReportAll(ref context);
    }

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
        TypeMustBePartialDiagnostic.CreateDescriptor(),
        ReturnModeOnUnsupportedAccessorDiagnostic.CreateDescriptor()
    );
}
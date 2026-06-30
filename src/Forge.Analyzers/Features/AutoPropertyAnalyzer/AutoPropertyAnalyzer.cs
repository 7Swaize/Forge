using System.Collections.Immutable;
using Forge.Analyzers.Common.Diagnostics;
using Forge.Analyzers.Common.Discovery;
using Forge.Analyzers.Features.AutoPropertyAnalyzer.Diagnostics;
using Forge.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Forge.Analyzers.Features.AutoPropertyAnalyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal sealed class AutoPropertyAnalyzer : DiagnosticAnalyzer {
    public override void Initialize(AnalysisContext context) {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(static compilationContext => {
            INamedTypeSymbol? targetAttr =
                compilationContext.Compilation.GetTypeByMetadataName(typeof(AutoPropertyAttribute).FullName!);

            if (targetAttr == null) {
                return;
            }

            compilationContext.RegisterSymbolAction(ctx => AnalyzeSymbol(ref ctx, targetAttr), SymbolKind.Field);
        });
    }

    private static void AnalyzeSymbol(ref SymbolAnalysisContext context, INamedTypeSymbol targetAttr) {
        if (context.Symbol is not IFieldSymbol field) {
            return;
        }

        if (!field.ValidateAnnotatedWith(targetAttr, out AttributeData? attribute)) {
            return;
        }

        Parse(field, attribute).ReportAll(ref context);
    }
    
    private static ImmutableArray<Diagnostic> Parse(IFieldSymbol field, AttributeData attribute) {
        ImmutableArray<Diagnostic>.Builder builder = ImmutableArray.CreateBuilder<Diagnostic>();

        Diagnostic? d1 = CommonDiagnosticParser.CheckContainingTypeMustBePartialDiagnostic(field);
        if (d1 != null) builder.Add(d1);
        
        Diagnostic? d2 = CheckReturnModeOnUnsupportedAccessorDiagnostic(field, attribute);
        if (d2 != null) builder.Add(d2);
        
        return builder.ToImmutable();
    }
    

    private static Diagnostic? CheckReturnModeOnUnsupportedAccessorDiagnostic(IFieldSymbol field, AttributeData attribute) {
        if (attribute.ConstructorArguments.Length < 3) {
            return null;
        }

        Accessors accessors = (Accessors)(byte)attribute.ConstructorArguments[1].Value!;
        ReturnMode returnMode = (ReturnMode)(byte)attribute.ConstructorArguments[2].Value!;

        bool emit = (returnMode is ReturnMode.RefStruct or ReturnMode.RefReadonlyStruct) &&
                    accessors is not Accessors.Get;

        if (emit) {
            return ReturnModeOnUnsupportedAccessorDiagnostic.CreateDiagnostic(
                field.Locations[0],
                returnMode,
                accessors
            );
        }
        
        return null;
    }

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
        TypeMustBePartialDiagnostic.CreateDescriptor(),
        ReturnModeOnUnsupportedAccessorDiagnostic.CreateDescriptor()
    );
}
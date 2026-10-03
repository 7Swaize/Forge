using System;
using System.Collections.Immutable;
using System.Globalization;
using Forge.Analyzers.Features.RemoveByAnalyzers.Diagnostics;
using Forge.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Forge.Analyzers.Features.RemoveByAnalyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal sealed class NoLocalsAttributeAnalyzer : DiagnosticAnalyzer {
    private const string KDateTimeFormat = "yyyy-MM-dd";
    
    public override void Initialize(AnalysisContext context) {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(static compilationContext => {
            INamedTypeSymbol? targetAttr =
                compilationContext.Compilation.GetTypeByMetadataName(typeof(NoLocalVariablesAttribute).FullName!);

            if (targetAttr == null) {
                return;
            }

            compilationContext.RegisterSymbolAction(
                ctx => AnalyzeTargetSymbol(ref ctx, targetAttr),
                SymbolKind.NamedType, SymbolKind.Method, SymbolKind.Field, SymbolKind.Property
            );
        });
    }

    private static void AnalyzeTargetSymbol(ref SymbolAnalysisContext ctx, INamedTypeSymbol targetAttr) {
        ISymbol targetSymbol = ctx.Symbol;
        
        if (!targetSymbol.ValidateAnnotatedWith(targetAttr, out AttributeData? attributeData)) {
            return;
        }

        string firstArgRaw = (string)attributeData.ConstructorArguments[0].Value!;
        
        if (!TryParseToDateTime(firstArgRaw, out DateTime usableDateTime)) {
            ctx.ReportDiagnostic(InvalidDateTimeFormatDiagnostic.CreateDiagnostic(
                attributeData.ApplicationSyntaxReference!.GetSyntax().GetLocation(),
                firstArgRaw
            ));
            return;
        }
        
        EmitDiagnosticRelativeToDate(ref ctx, usableDateTime, targetSymbol);
    }

    private static bool TryParseToDateTime(string userInput, out DateTime usableDateTime) {
        if (DateTime.TryParseExact(userInput, KDateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime safeDate)) {
            usableDateTime = safeDate;
            return true;
        }
        
        usableDateTime = DateTime.MinValue;
        return false;
    }

    private static void EmitDiagnosticRelativeToDate(ref SymbolAnalysisContext ctx, DateTime dateTime, ISymbol targetSymbol) {
        if (dateTime > DateTime.Today) {
            ctx.ReportDiagnostic(PreRemoveByDateDiagnostic.CreateDiagnostic(
                targetSymbol.Locations[0],
                dateTime.ToString(KDateTimeFormat, CultureInfo.InvariantCulture)
            ));
        }
        
        ctx.ReportDiagnostic(PostRemoveByDataDiagnostic.CreateDiagnostic(
            targetSymbol.Locations[0],
            dateTime.ToString(KDateTimeFormat, CultureInfo.InvariantCulture)
        ));
    }

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
        PreRemoveByDateDiagnostic.CreateDescriptor(),
        PostRemoveByDataDiagnostic.CreateDescriptor(),
        InvalidDateTimeFormatDiagnostic.CreateDescriptor()
    );
}
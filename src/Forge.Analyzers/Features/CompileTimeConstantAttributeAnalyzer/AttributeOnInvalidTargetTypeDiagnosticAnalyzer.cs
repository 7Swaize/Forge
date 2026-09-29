using System.Collections.Immutable;
using Forge.Analyzers.Features.CompileTimeConstantAttribute.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Forge.Analyzers.Features.CompileTimeConstantAnalyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal sealed class AttributeOnInvalidTargetTypeDiagnosticAnalyzer : DiagnosticAnalyzer {
    public override void Initialize(AnalysisContext context) {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        
        context.RegisterCompilationStartAction(static compilationContext => {
            INamedTypeSymbol? targetAttr =
                compilationContext.Compilation.GetTypeByMetadataName(typeof(Annotations.CompileTimeConstantAttribute).FullName!);

            if (targetAttr == null) {
                return;
            }

            compilationContext.RegisterSymbolAction(ctx => AnalyzeOperation(ref ctx, targetAttr), SymbolKind.Parameter);
        });
    }

    private static void AnalyzeOperation(ref SymbolAnalysisContext ctx, INamedTypeSymbol targetAttr) {
        IParameterSymbol param = (IParameterSymbol)ctx.Symbol;

        if (!param.ValidateAnnotatedWith(targetAttr)) {
            return;
        }

        if (TypeCanBeCompileTimeConstant(param.Type)) {
            return;
        }
        
        ctx.ReportDiagnostic(AttributeOnInvalidTargetTypeDiagnostic.CreateDiagnostic(
            param.DeclaringSyntaxReferences[0].GetSyntax().GetLocation(),
            param.Type
        ));
    }

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
        AttributeOnInvalidTargetTypeDiagnostic.CreateDescriptor()
    );
    
    private static bool TypeCanBeCompileTimeConstant(ITypeSymbol type) {
        if (type.TypeKind == TypeKind.Enum) {
            return true;
        }

        return type.SpecialType switch {
            SpecialType.System_Boolean or SpecialType.System_Char or SpecialType.System_SByte or SpecialType.System_Byte
                or SpecialType.System_Int16 or SpecialType.System_UInt16 or SpecialType.System_Int32
                or SpecialType.System_UInt32 or SpecialType.System_Int64 or SpecialType.System_UInt64
                or SpecialType.System_Single or SpecialType.System_Double or SpecialType.System_Decimal
                or SpecialType.System_String => true,
            _ => false
        };
    }
}
using System;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using Forge.Analyzers.Features.BlittableAnalyzers.Diagnostics;
using Forge.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Forge.Analyzers.Features.BlittableAnalyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal sealed class BlittableAttributeAnalyzer : DiagnosticAnalyzer {
    public override void Initialize(AnalysisContext context) {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        
        context.RegisterCompilationStartAction(static compilationContext => {
            INamedTypeSymbol? targetAttr = compilationContext.Compilation.GetTypeByMetadataName(typeof(BlittableAttribute).FullName!);

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

        INamedTypeSymbol? structLayoutAttr = ctx.Compilation.GetTypeByMetadataName(typeof(StructLayoutAttribute).FullName!);

        // Just checks the struct layout
        if (structLayoutAttr is not null && target.ValidateAnnotatedWith(structLayoutAttr, out AttributeData? structLayoutAttrData)) {
            if (structLayoutAttrData.ConstructorArguments.Length > 0) {
                TypedConstant layoutArg = structLayoutAttrData.ConstructorArguments[0];

                if (layoutArg.Value is not null) {
                    LayoutKind layoutKind = (LayoutKind)Convert.ToInt32(layoutArg.Value);

                    if (layoutKind == LayoutKind.Auto) {
                        ctx.ReportDiagnostic(IncompatibleLayoutDiagnostic.CreateDiagnostic(
                            target.Locations[0],
                            target
                        ));
                    }
                }
            }
        }

        foreach (IFieldSymbol field in target.GetMembers().OfType<IFieldSymbol>()) {
            if (field.IsStatic) {
                continue;
                
            }
            
            if (IsBlittable(field.Type)) {
                continue;
            }
            
            ctx.ReportDiagnostic(ContainedTypeNotBlittableDiagnostic.CreateDiagnostic(
                field.Locations[0],
                field.Type
            ));
        }
    }

    private static bool IsBlittable(ITypeSymbol type) {
        if (IsPrimitiveBlittable(type)) {
            return true;
        }
        
        if (type.TypeKind is TypeKind.Enum or TypeKind.Pointer or TypeKind.FunctionPointer) {
            return true;
        }
        
        if (type.NullableAnnotation == NullableAnnotation.Annotated) {
            return false;
        }

        if (type.SpecialType is SpecialType.System_Char or SpecialType.System_Boolean or SpecialType.System_Decimal) {
            return false;
        }
        
        if (type is not INamedTypeSymbol { TypeKind: TypeKind.Struct } named) {
            return false;
        }
        
        foreach (IFieldSymbol field in named.GetMembers().OfType<IFieldSymbol>()) {
            if (field.IsStatic) {
                continue;
            }
            
            if (field.IsFixedSizeBuffer) {
                if (!IsBlittable(field.Type.As<IPointerTypeSymbol>().PointedAtType)) {
                    return false;
                }
            }

            if (!IsBlittable(field.Type)) {
                return false;
            }
        }
        
        return true;
    }

    private static bool IsPrimitiveBlittable(ITypeSymbol typeSymbol) {
        return typeSymbol.SpecialType switch {
            SpecialType.System_Byte => true,
            SpecialType.System_SByte => true,
            SpecialType.System_Int16 => true,
            SpecialType.System_UInt16 => true,
            SpecialType.System_Int32 => true,
            SpecialType.System_UInt32 => true,
            SpecialType.System_Int64 => true,
            SpecialType.System_UInt64 => true,
            SpecialType.System_IntPtr => true,
            SpecialType.System_UIntPtr => true,
            SpecialType.System_Single => true,
            SpecialType.System_Double => true,
            _ => false
        };
    }

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
        ContainedTypeNotBlittableDiagnostic.CreateDescriptor(),
        IncompatibleLayoutDiagnostic.CreateDescriptor()
    );
}
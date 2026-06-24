using System.Collections.Immutable;
using Forge.Analyzers.Common.Discovery;
using Forge.Analyzers.Features.AutoPropertyAnalyzer.Diagnostics;
using Forge.Annotations;
using Microsoft.CodeAnalysis;

namespace Forge.Analyzers.Features.AutoPropertyAnalyzer.Discovery;

internal static class ParseAutoPropertyAttribute {
    internal static ImmutableArray<Diagnostic> Parse(IFieldSymbol field, AttributeData attribute) {
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
}
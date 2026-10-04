using Forge.Annotations;
using Forge.Generators.Common.Models.Factories;
using Forge.Generators.Features.ZeroCopyParseGenerators.Models;
using Forge.RoslynShared;
using Microsoft.CodeAnalysis;

namespace Forge.Generators.Features.ZeroCopyParseGenerators.Discovery;

internal static class GeneratorTargetParser {
    internal static EndianKind GetEndianness(in GeneratorAttributeSyntaxContext ctx) {
        INamedTypeSymbol? targetAttr =
            ctx.SemanticModel.Compilation.GetTypeByMetadataName(typeof(ZeroCopyParserAttribute).FullName!);

        foreach (AttributeData attr in ctx.Attributes) {
            if (SymbolEqualityComparer.Default.Equals(attr.AttributeClass, targetAttr)) {
                return (EndianKind)(int)attr.ConstructorArguments[0].Value!;
            }
        }
        
        return ThrowHelpers.ThrowUnreachable<EndianKind>();
    }

    internal static bool GetDisableReorderOptimizationsFlag(in GeneratorAttributeSyntaxContext ctx) {
        INamedTypeSymbol? targetAttr =
            ctx.SemanticModel.Compilation.GetTypeByMetadataName(typeof(DisableParseOrderOptimizationsAttribute).FullName!);

        foreach (AttributeData attr in ctx.Attributes) {
            if (SymbolEqualityComparer.Default.Equals(attr.AttributeClass, targetAttr)) {
                return true;
            }
        }

        return false;
    }

    internal static FieldTargetModel ParseField(IFieldSymbol fieldSymbol, TypeReferenceModelFactory typeRefFactory) {
        
    }
}
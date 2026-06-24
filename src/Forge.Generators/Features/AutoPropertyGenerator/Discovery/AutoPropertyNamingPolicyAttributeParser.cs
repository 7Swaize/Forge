using Forge.Annotations;
using Microsoft.CodeAnalysis;

namespace Forge.Generators.Features.AutoPropertyGenerator.Discovery;

internal static class AutoPropertyNamingPolicyAttributeParser {
    internal static NamingPolicy Parse(in GeneratorAttributeSyntaxContext context) {
        INamedTypeSymbol? targetAttr =
            context.SemanticModel.Compilation.GetTypeByMetadataName(typeof(AutoPropertyNamingPolicyAttribute).FullName!);

        foreach (AttributeData attr in context.Attributes) {
            if (!SymbolEqualityComparer.Default.Equals(attr.AttributeClass, targetAttr)) {
                continue;
            }

            return (NamingPolicy)(int)attr.ConstructorArguments[0].Value!;
        }
        
        return default;
    }
}
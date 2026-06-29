using Forge.Annotations;
using Forge.Generators.Features.AutoPropertyGenerator.Models;
using Microsoft.CodeAnalysis;

namespace Forge.Generators.Features.AutoPropertyGenerator.Discovery;

internal static class AutoPropertyAttributeParser {
    internal static AutoPropertyAttributeArgs ParseArgs(in GeneratorAttributeSyntaxContext context) {
        INamedTypeSymbol? targetAttr =
            context.SemanticModel.Compilation.GetTypeByMetadataName(typeof(AutoPropertyAttribute).FullName!);

        foreach (AttributeData attr in context.Attributes) {
            if (!SymbolEqualityComparer.Default.Equals(attr.AttributeClass, targetAttr)) {
                continue;
            }

            byte visibility = (byte)attr.ConstructorArguments[0].Value!;
            byte accessors = (byte)attr.ConstructorArguments[1].Value!;
            byte returnMode = 0;

            if (attr.ConstructorArguments.Length > 2) {
                returnMode = (byte)attr.ConstructorArguments[2].Value!;
            }

            return new AutoPropertyAttributeArgs {
                TargetVisibility = (Visibility)visibility,
                TargetAccessors = (Accessors)accessors,
                TargetReturnMode = (ReturnMode)returnMode
            };
        }
        
        return default;
    }
}
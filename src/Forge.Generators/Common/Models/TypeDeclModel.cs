using Forge.Generators.Common.Models.Factories;
using Microsoft.CodeAnalysis;

namespace Forge.Generators.Common.Models;

internal record TypeDeclModel {
    internal TypeDeclModel(INamedTypeSymbol symbol) {
        AsTypeRef = TypeReferenceModelFactory.CreateOrGetTypeReferenceModel(symbol);
        AccessModifier = symbol.DeclaredAccessibility;
        IsPartial = symbol.IsPartialDeclaration();
        IsStatic = symbol.IsStatic;
        IsSealed = symbol.IsSealed;
        
        FQNNoGlobal = symbol.GetConstructedTypeFQN(false);
    }
    
    internal ITypeReferenceModel AsTypeRef { get; init; }
    internal Accessibility AccessModifier { get; init; }
    internal bool IsPartial { get; init; }
    internal bool IsStatic { get; init; }
    internal bool IsSealed { get; init; }
    
    internal string FQNNoGlobal { get; init; }
}
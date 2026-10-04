using Forge.Generators.Common.Models.Factories;
using Microsoft.CodeAnalysis;

namespace Forge.Generators.Common.Models;

internal sealed record FieldModel {
    internal FieldModel(IFieldSymbol field, TypeReferenceModelFactory typeRefFactory) {
        Type = typeRefFactory.CreateOrGetTypeReferenceModel(field.Type);
        FieldName = field.Name;
        
        IsStatic = field.IsStatic;
        IsConst = field.IsConst;
    }
    
    internal ITypeReferenceModel Type { get; init; }
    internal string FieldName { get; init; }
    
    internal bool IsStatic { get; init; }
    internal bool IsConst { get; init; }
}
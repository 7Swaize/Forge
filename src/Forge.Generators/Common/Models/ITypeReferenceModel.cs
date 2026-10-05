using System;
using Forge.Generators.Common.Models.Collections;
using Microsoft.CodeAnalysis;

namespace Forge.Generators.Common.Models;

internal interface ITypeReferenceModel : IEquatable<ITypeReferenceModel> {
    string FQNGenericOmitted { get; init; }
    string FQNGenericBased { get; init; }
    string FQNArityBased { get; init; }
    string FQNConstructedArgBased { get; init; }
    string FlattenedNameArityBased { get; init; }
    string? Namespace { get; init; }
    
    bool IsBasedOnTypeParameter { get; init; }
    bool IsGeneric { get; init; }
    bool IsUnboundGeneric { get; init; }
    ITypeReferenceModel? UnboundGenericTypeRef { get; init; }
    EquatableArray<ITypeReferenceModel> TypeArguments { get; init; }
    EquatableArray<ITypeReferenceModel> TypeParameters { get; init; }
    EquatableArray<ConstraintsModel> Constraints { get; init; }
    
    bool IsTrueValueType { get; init; }
    TypeKind TypeKind { get; init; }
    SpecialType SpecialType { get; init; }
    
    ITypeReferenceModel? BaseType { get; init; }
    EquatableArray<ITypeReferenceModel> ImmediateInterfaces { get; init; }
    EquatableArray<ITypeReferenceModel> AllInterfaces { get; init; }
    
    ITypeSymbol UnderlyingTypeSymbol { get; }
}
using Forge.Annotations;
using Forge.Generators.Common.Models;
using Forge.Generators.Common.Models.Factories;
using Forge.Generators.Features.AutoPropertyGenerator.Discovery;
using Microsoft.CodeAnalysis;

namespace Forge.Generators.Features.AutoPropertyGenerator.Models;

internal sealed record TargetFieldModel {
    internal TargetFieldModel(in GeneratorAttributeSyntaxContext context) {
        TypeReferenceModelFactory typeRefFactory = TypeReferenceModelFactory.GetFactory(context.SemanticModel.Compilation);
        
        ContainingTypeDecl = new TypeDeclModel(context.TargetSymbol.ContainingType, typeRefFactory);
        
        IFieldSymbol target = (IFieldSymbol)context.TargetSymbol;
        FieldType = typeRefFactory.CreateOrGetTypeReferenceModel(target.Type);
        Name = target.Name;

        AutoPropertyAttributeArgs args = AutoPropertyAttributeParser.ParseArgs(in context);
        TargetVisibility = args.TargetVisibility;
        TargetAccessors = args.TargetAccessors;
        TargetReturnMode = args.TargetReturnMode;
    }
    
    internal TypeDeclModel ContainingTypeDecl { get; init; }
        
    internal ITypeReferenceModel FieldType { get; init; }
    internal string Name { get; init; }
    internal Visibility TargetVisibility { get; init; }
    internal Accessors TargetAccessors { get; init; }
    internal ReturnMode TargetReturnMode { get; init; }
}


internal readonly struct AutoPropertyAttributeArgs {
    internal Visibility TargetVisibility { get; init; }
    internal Accessors TargetAccessors { get; init; }
    internal ReturnMode TargetReturnMode { get; init; }
}
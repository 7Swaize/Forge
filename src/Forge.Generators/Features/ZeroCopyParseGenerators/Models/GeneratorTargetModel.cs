using System.Linq;
using System.Runtime.InteropServices;
using Forge.Generators.Common.Models;
using Forge.Generators.Common.Models.Collections;
using Forge.Generators.Common.Models.Factories;
using Forge.Generators.Features.ZeroCopyParseGenerators.Discovery;
using Microsoft.CodeAnalysis;

namespace Forge.Generators.Features.ZeroCopyParseGenerators.Models;

internal sealed record GeneratorTargetModel {
    internal GeneratorTargetModel(in GeneratorAttributeSyntaxContext context) {
        TypeReferenceModelFactory typeRefFactory = TypeReferenceModelFactory.GetFactory(context.SemanticModel.Compilation);
        INamedTypeSymbol targetSymbol = (INamedTypeSymbol)context.TargetSymbol;
        
        TypeDecl = new TypeDeclModel(targetSymbol, typeRefFactory);
        
        Endianness = GeneratorTargetParser.GetEndianness(in context);
        DisableReorderOptimizations = GeneratorTargetParser.GetDisableReorderOptimizationsFlag(in context);

        TargetFields = targetSymbol
            .GetMembers()
            .OfType<IFieldSymbol>()
            .Select(fs => GeneratorTargetParser.ParseField(fs, typeRefFactory))
            .ToArray()
            .AsImmutableArrayUnsafe()
            .AsEquatableArray();
        
        AggregateModifierDescriptor = TargetFields
            .AsArrayUnsafe()
            .SelectMany(fieldTarget => fieldTarget.Modifiers.AsArrayUnsafe())
            .Aggregate(FieldModifierKind.None, (current, field) => current | field.Kind);
        
        IsBlittable = GeneratorTargetParser.GetIsBlittable(in context);
        (LayoutKind, Pack) = GeneratorTargetParser.GetStructLayoutInformation(in context, IsBlittable);
    }
    
    internal TypeDeclModel TypeDecl { get; init; }
    
    internal EndianKind Endianness { get; init; }
    internal bool DisableReorderOptimizations { get; init; }
    
    internal LayoutKind LayoutKind { get; init; }
    internal bool IsBlittable { get; init; }
    internal int Pack { get; init; }
    
    internal FieldModifierKind AggregateModifierDescriptor { get; set; }
    internal EquatableArray<FieldTargetModel> TargetFields { get; set; }
}


// Equivalent to public API enum
internal enum EndianKind {
    Big = 0,
    Little = 1
}

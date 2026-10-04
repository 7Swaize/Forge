using System.Linq;
using Forge.Generators.Common.Models.Collections;
using Forge.Generators.Common.Models.Factories;
using Forge.Generators.Features.ZeroCopyParseGenerators.Discovery;
using Microsoft.CodeAnalysis;

namespace Forge.Generators.Features.ZeroCopyParseGenerators.Models;

internal sealed record GeneratorTargetModel {
    internal GeneratorTargetModel(in GeneratorAttributeSyntaxContext context) {
        TypeReferenceModelFactory typeRefFactory = TypeReferenceModelFactory.GetFactory(context.SemanticModel.Compilation);
        
        Endianness = GeneratorTargetParser.GetEndianness(in context);
        DisableReorderOptimizations = GeneratorTargetParser.GetDisableReorderOptimizationsFlag(in context);

        TargetFields = context.TargetSymbol
            .As<INamedTypeSymbol>()
            .GetMembers()
            .OfType<IFieldSymbol>()
            .Select(fs => GeneratorTargetParser.ParseField(fs, typeRefFactory))
            .ToArray()
            .AsImmutableArrayUnsafe()
            .AsEquatableArray();
        
        AggregateModifierDescriptor = TargetFields
            .AsArrayUnsafe()
            .Aggregate(FieldModifierKind.None, (current, field) => current | field.Kind);
    }
    
    internal EndianKind Endianness { get; init; }
    internal bool DisableReorderOptimizations { get; init; }
    
    internal FieldModifierKind AggregateModifierDescriptor { get; set; }
    internal EquatableArray<FieldTargetModel> TargetFields { get; set; }
}


// Equivalent to public API enum
internal enum EndianKind {
    Big = 0,
    Little = 1
}

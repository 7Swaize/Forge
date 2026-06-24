using System.Collections.Generic;
using System.Collections.Immutable;
using Forge.Annotations;
using Forge.Generators.Common.Models;
using Forge.Generators.Features.AutoPropertyGenerator.Discovery;
using Forge.Generators.Features.AutoPropertyGenerator.Emit;
using Forge.Generators.Features.AutoPropertyGenerator.Models;
using Microsoft.CodeAnalysis;

namespace Forge.Generators.Features.AutoPropertyGenerator;

[Generator]
internal sealed class AutoPropertyGenerator : IIncrementalGenerator {
    public void Initialize(IncrementalGeneratorInitializationContext context) {
        IncrementalValuesProvider<TargetFieldModel> fields = context.SyntaxProvider.ForAttributeWithMetadataName(
            fullyQualifiedMetadataName: typeof(AutoPropertyAttribute).FullName!,
            predicate: static (_, _) => true,
            transform: static (context, _) => new TargetFieldModel(in context
        ));

        IncrementalValuesProvider<GroupTargetModel> groupings = fields.Collect().SelectMany(static (all, _) => {
            Dictionary<TypeDeclModel, List<TargetFieldModel>> map = new();

            foreach (TargetFieldModel field in all) {
                if (map.GetValueOrDefault(field.ContainingTypeDecl) is { } collection) {
                    collection.Add(field);
                    continue;
                }

                map[field.ContainingTypeDecl] = [
                    field
                ];
            }

            GroupTargetModel[] groups = new GroupTargetModel[map.Count];

            int i = 0;
            foreach (KeyValuePair<TypeDeclModel, List<TargetFieldModel>> kvp in map) {
                groups[i++] = new GroupTargetModel(
                    kvp.Key,
                    kvp.Value.ToImmutableArray()
                );
            }
            
            return groups;
        });

        IncrementalValueProvider<NamingPolicy> namingPolicy = context.SyntaxProvider.ForAttributeWithMetadataName(
            fullyQualifiedMetadataName: typeof(AutoPropertyNamingPolicyAttribute).FullName!,
            predicate: static (_, _) => true,
            transform: static (context, _) => AutoPropertyNamingPolicyAttributeParser.Parse(in context)
        ).Collect().Select(static (all, _) => all[0]);
        
        IncrementalValuesProvider<(GroupTargetModel Left, NamingPolicy Right)> combined = groupings.Combine(namingPolicy);
        
        context.RegisterSourceOutput(combined, static (ctx, tuple) => {
            EmitAutoProperty.Emit(tuple.Left, tuple.Right);
        });
    }
}
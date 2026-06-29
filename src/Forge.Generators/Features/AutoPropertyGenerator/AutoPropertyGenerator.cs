using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Forge.Annotations;
using Forge.Generators.Common.Models;
using Forge.Generators.Features.AutoPropertyGenerator.Discovery;
using Forge.Generators.Features.AutoPropertyGenerator.Emit;
using Forge.Generators.Features.AutoPropertyGenerator.Models;
using Microsoft.CodeAnalysis;
using GeneratedSource = (string name, Microsoft.CodeAnalysis.Text.SourceText sourceText);

[assembly: InternalsVisibleTo("Forge.Tests")]

namespace Forge.Generators.Features.AutoPropertyGenerator;

[Generator]
internal sealed class AutoPropertyGenerator : IIncrementalGenerator {
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    internal static class TrackingNames {
        internal const string Fields = nameof(Fields);
        internal const string Groupings = nameof(Groupings);
        internal const string NamingPolicy = nameof(NamingPolicy);
        internal const string Combined = nameof(Combined);
    }
    
    public void Initialize(IncrementalGeneratorInitializationContext context) {
        IncrementalValuesProvider<TargetFieldModel> fields = context.SyntaxProvider.ForAttributeWithMetadataName(
            fullyQualifiedMetadataName: typeof(AutoPropertyAttribute).FullName!,
            predicate: static (_, _) => true,
            transform: static (context, _) => new TargetFieldModel(in context)
        )
        .WithTrackingName(TrackingNames.Fields);

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
        })
        .WithTrackingName(TrackingNames.Groupings);

        IncrementalValueProvider<NamingPolicy> namingPolicy = context.SyntaxProvider.ForAttributeWithMetadataName(
            fullyQualifiedMetadataName: typeof(AutoPropertyNamingPolicyAttribute).FullName!,
            predicate: static (_, _) => true,
            transform: static (context, _) => AutoPropertyNamingPolicyAttributeParser.Parse(in context)
        )
        .Collect()
        .Select(static (all, _) => all.FirstOrDefault())
        .WithTrackingName(TrackingNames.NamingPolicy);
        
        IncrementalValuesProvider<(GroupTargetModel Left, NamingPolicy Right)> combined = 
            groupings
                .Combine(namingPolicy)
                .WithTrackingName(TrackingNames.Combined);
        
        context.RegisterSourceOutput(combined, static (ctx, tuple) => {
            GeneratedSource source = EmitAutoProperty.Emit(tuple.Left, tuple.Right);
            
            ctx.AddSource(source.name, source.sourceText);
        });
    }
}
using System.Diagnostics.CodeAnalysis;
using Forge.Annotations;
using Forge.Generators.Features.ZeroCopyParseGenerators.Models;
using Microsoft.CodeAnalysis;

namespace Forge.Generators.Features.ZeroCopyParseGenerators;

[Generator]
internal sealed class ZeroCopyParserGenerator : IIncrementalGenerator {
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    internal static class TrackingNames {
        internal const string ParseGeneratorTargets = nameof(ParseGeneratorTargets); 
    }
    
    public void Initialize(IncrementalGeneratorInitializationContext context) {
        IncrementalValuesProvider<GeneratorTargetModel> targets = context.SyntaxProvider.ForAttributeWithMetadataName(
            fullyQualifiedMetadataName: typeof(ZeroCopyParserAttribute).FullName!,
            predicate: (_, _) => true,
            transform: (ctx, ct) => {
                ct.ThrowIfCancellationRequested();
                return new GeneratorTargetModel(in ctx);
            }
        ).WithTrackingName(TrackingNames.ParseGeneratorTargets);
    }
}
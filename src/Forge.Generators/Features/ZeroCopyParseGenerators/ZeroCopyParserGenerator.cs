using System.Diagnostics.CodeAnalysis;
using Forge.Annotations;
using Forge.Generators.Features.ZeroCopyParseGenerators.Emit;
using Forge.Generators.Features.ZeroCopyParseGenerators.Models;
using Microsoft.CodeAnalysis;
using GeneratedSource = (string name, Microsoft.CodeAnalysis.Text.SourceText sourceText);

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
        
        context.RegisterSourceOutput(targets, (ctx, target) => {
            ctx.CancellationToken.ThrowIfCancellationRequested();
            
            GeneratedSource source = EmitParser.Emit(target);
            ctx.AddSource(source.name, source.sourceText);
        });
    }
}
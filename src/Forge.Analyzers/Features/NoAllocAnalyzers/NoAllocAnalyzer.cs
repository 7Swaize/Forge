using System.Collections.Immutable;
using Forge.Analyzers.Features.NoAllocAnalyzers.Diagnostics;
using Forge.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Forge.Analyzers.Features.NoAllocAnalyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal sealed class NoAllocAnalyzer : DiagnosticAnalyzer {
    public override void Initialize(AnalysisContext context) {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(static compilationContext => {
            INamedTypeSymbol? targetAttr =
                compilationContext.Compilation.GetTypeByMetadataName(typeof(NoAllocAttribute).FullName!);

            if (targetAttr == null) {
                return;
            }


        });
    }

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
        BoxingAllocationDiagnostic.CreateDescriptor(),
        ClosureAllocationDiagnostic.CreateDescriptor(),
        GetEnumeratorAllocationDiagnostic.CreateDescriptor(),
        HeapAllocationObjectCreationDiagnostic.CreateDescriptor(),
        ParamsArrayAllocationDiagnostic.CreateDescriptor(),
        StateMachineAllocationDiagnostic.CreateDescriptor()
    );
}
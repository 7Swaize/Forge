using System.Collections.Immutable;
using Forge.Analyzers.Features.CompileTimeConstantAttribute.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Forge.Analyzers.Features.CompileTimeConstantAnalyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal sealed class RequiresCompileTimeConstantDiagnosticAnalyzer : DiagnosticAnalyzer {
    public override void Initialize(AnalysisContext context) {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(static compilationContext => {
            INamedTypeSymbol? targetAttr =
                compilationContext.Compilation.GetTypeByMetadataName(typeof(Annotations.CompileTimeConstantAttribute).FullName!);

            if (targetAttr == null) {
                return;
            }

            compilationContext.RegisterOperationAction(ctx => AnalyzeOperation(ref ctx, targetAttr), OperationKind.Invocation);
        });
    }

    private static void AnalyzeOperation(ref OperationAnalysisContext ctx, INamedTypeSymbol targetAttr) {
        IInvocationOperation invocation = (IInvocationOperation)ctx.Operation;

        foreach (IArgumentOperation argument in invocation.Arguments) {
            if (argument.Parameter == null || !argument.Parameter.ValidateAnnotatedWith(targetAttr)) {
                continue;
            }

            if (argument.Value.ConstantValue.HasValue) {
                continue;
            }
            
            ctx.ReportDiagnostic(RequiresCompileTimeConstantDiagnostic.CreateDiagnostic(
                argument.Syntax.GetLocation(),
                argument.Parameter.Name
            ));
        }
    }

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
        RequiresCompileTimeConstantDiagnostic.CreateDescriptor()
    );
}
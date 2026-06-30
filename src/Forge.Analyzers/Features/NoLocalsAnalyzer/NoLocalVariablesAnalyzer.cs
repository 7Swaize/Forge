using System.Collections.Immutable;
using System.Linq;
using Forge.Analyzers.Features.NoLocalsAnalyzer.Diagnostics;
using Forge.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Forge.Analyzers.Features.NoLocalsAnalyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal sealed class NoLocalVariablesAnalyzer : DiagnosticAnalyzer {
    public override void Initialize(AnalysisContext context) {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        
        context.RegisterCompilationStartAction(static compilationContext => {
            INamedTypeSymbol? targetAttr =
                compilationContext.Compilation.GetTypeByMetadataName(typeof(NoLocalVariablesAttribute).FullName!);

            if (targetAttr == null) {
                return;
            }
            
            compilationContext.RegisterSymbolAction(ctx => AnalyzeSymbol(ref ctx, targetAttr), SymbolKind.Method);
        });
    }

    private static void AnalyzeSymbol(ref SymbolAnalysisContext context, INamedTypeSymbol targetAttr) {
        if (context.Symbol is not IMethodSymbol methodType) {
            return;
        }
        
        if (!methodType.ValidateAnnotatedWith(targetAttr)) {
            return;
        }

        foreach (SyntaxReference syntaxRef in methodType.DeclaringSyntaxReferences) {
            SyntaxNode syntax = syntaxRef.GetSyntax();
            
            if (syntax is not MethodDeclarationSyntax) {
                continue;
            }

            SyntaxNode? body = syntax switch {
                MethodDeclarationSyntax m => m.Body ?? (SyntaxNode?)m.ExpressionBody?.Expression,
                ConstructorDeclarationSyntax c => c.Body ?? (SyntaxNode?)c.ExpressionBody?.Expression,
                DestructorDeclarationSyntax d => d.Body ?? (SyntaxNode?)d.ExpressionBody?.Expression,
                OperatorDeclarationSyntax o => o.Body ?? (SyntaxNode?)o.ExpressionBody?.Expression,
                ConversionOperatorDeclarationSyntax c => c.Body ?? (SyntaxNode?)c.ExpressionBody?.Expression,
                AccessorDeclarationSyntax a => a.Body ?? (SyntaxNode?)a.ExpressionBody?.Expression,
                LocalFunctionStatementSyntax l => l.Body ?? (SyntaxNode?)l.ExpressionBody?.Expression,
                AnonymousFunctionExpressionSyntax a => a.Body,
                _ => null
            };
            
            body?.DescendantNodes(descendIntoChildren: node => node switch {
                LocalFunctionStatementSyntax => false,
                AnonymousFunctionExpressionSyntax => false,
                _ => true
            })
            .OfType<LocalDeclarationStatementSyntax>()
            .Select(local => MethodCannotContainLocalVariableDeclarationsDiagnostic.CreateDiagnostic(local.GetLocation()))
            .ReportAll(ref context);
        }
    }

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
        MethodCannotContainLocalVariableDeclarationsDiagnostic.CreateDescriptor()
    );
}
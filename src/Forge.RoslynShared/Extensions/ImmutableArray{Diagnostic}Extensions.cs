using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

public static class ImmutableArrayDiagnosticExtensions {
    extension(ImmutableArray<Diagnostic> self) {
        public void ReportAll(ref SymbolAnalysisContext context) {
            foreach (Diagnostic diagnostic in self) {
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
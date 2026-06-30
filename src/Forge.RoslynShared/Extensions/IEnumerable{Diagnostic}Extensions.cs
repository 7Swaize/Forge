using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

public static class IEnumerableDiagnosticExtensions {
    extension(IEnumerable<Diagnostic> self) {
        public void ReportAll(ref SymbolAnalysisContext context) {
            foreach (Diagnostic diagnostic in self) {
                context.ReportDiagnostic(diagnostic);
            }
        }

        public void ReportAll(ref SyntaxNodeAnalysisContext context) {
            foreach (Diagnostic diagnostic in self) {
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
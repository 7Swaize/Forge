using System;
using System.Collections.Immutable;
using System.Globalization;
using Forge.Analyzers.Features.RemoveByAnalyzers.Diagnostics;
using Forge.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Forge.Analyzers.Features.RemoveByAnalyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal sealed class RemoveByAttributeAnalyzer : DiagnosticAnalyzer {
    private const string KRemoveByAttribute = "RemoveBy";
    private const string KDateTimeFormat = "yyyy-MM-dd";

    public override void Initialize(AnalysisContext context) {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(static compilationContext => {
            INamedTypeSymbol? targetAttr =
                compilationContext.Compilation.GetTypeByMetadataName(typeof(RemoveByAttribute).FullName!);

            if (targetAttr == null) {
                return;
            }

            compilationContext.RegisterSyntaxNodeAction(ctx => AnalyzeAttribute(ref ctx, targetAttr), SyntaxKind.Attribute);
        });
    }

    private static void AnalyzeAttribute(ref SyntaxNodeAnalysisContext ctx, INamedTypeSymbol targetAttr) {
        AttributeSyntax attributeSyntax = (AttributeSyntax)ctx.Node;

        if (!attributeSyntax.Name.ToString().Contains(KRemoveByAttribute)) {
            return;
        }

        SymbolInfo symbolInfo = ctx.SemanticModel.GetSymbolInfo(attributeSyntax, ctx.CancellationToken);
        if (symbolInfo.Symbol?.ContainingType is not { } attributeType ||
            !SymbolEqualityComparer.Default.Equals(attributeType, targetAttr)) {
            return;
        }

        AttributeArgumentListSyntax? argumentList = attributeSyntax.ArgumentList;
        if (argumentList == null || argumentList.Arguments.Count == 0) {
            return;
        }

        ExpressionSyntax firstArgExpression = argumentList.Arguments[0].Expression;
        Optional<object?> constantValue = ctx.SemanticModel.GetConstantValue(firstArgExpression, ctx.CancellationToken);

        if (!constantValue.HasValue || constantValue.Value is not string firstArgRaw) {
            return;
        }

        if (!TryParseToDateTime(firstArgRaw, out DateTime usableDateTime)) {
            ctx.ReportDiagnostic(InvalidDateTimeFormatDiagnostic.CreateDiagnostic(
                attributeSyntax.GetLocation(),
                firstArgRaw
            ));
            return;
        }

        SyntaxNode? attributedNode = attributeSyntax.Parent?.Parent;

        if (attributedNode != null) {
            if (attributedNode is FieldDeclarationSyntax fieldDeclaration) {
                foreach (VariableDeclaratorSyntax decl in fieldDeclaration.Declaration.Variables) {
                    Location targetLocation = GetTargetLocation(ref ctx, decl);
                    EmitDiagnosticRelativeToDate(ref ctx, usableDateTime, targetLocation);
                }
            }
            else {
                Location targetLocation = GetTargetLocation(ref ctx, attributedNode);
                EmitDiagnosticRelativeToDate(ref ctx, usableDateTime, targetLocation);
            }
        }

    }

    private static bool TryParseToDateTime(string userInput, out DateTime usableDateTime) {
        if (DateTime.TryParseExact(userInput, KDateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime safeDate)) {
            usableDateTime = safeDate;
            return true;
        }

        usableDateTime = DateTime.MinValue;
        return false;
    }
    
    private static Location GetTargetLocation(ref SyntaxNodeAnalysisContext ctx, SyntaxNode decl) {
        ISymbol? declaredSymbol = ctx.SemanticModel.GetDeclaredSymbol(decl, ctx.CancellationToken);
        Location targetLocation = declaredSymbol is { Locations.Length: > 0 }
            ? declaredSymbol.Locations[0]
            : decl.GetLocation();
        
        return targetLocation;
    }

    private static void EmitDiagnosticRelativeToDate(ref SyntaxNodeAnalysisContext ctx, DateTime dateTime, Location location) {
        if (dateTime > DateTime.Today) {
            ctx.ReportDiagnostic(PreRemoveByDateDiagnostic.CreateDiagnostic(
                location,
                dateTime.ToString(KDateTimeFormat, CultureInfo.InvariantCulture)
            ));
            return;
        }

        ctx.ReportDiagnostic(PostRemoveByDataDiagnostic.CreateDiagnostic(
            location,
            dateTime.ToString(KDateTimeFormat, CultureInfo.InvariantCulture)
        ));
    }

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
        PreRemoveByDateDiagnostic.CreateDescriptor(),
        PostRemoveByDataDiagnostic.CreateDescriptor(),
        InvalidDateTimeFormatDiagnostic.CreateDescriptor()
    );
}
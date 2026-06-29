using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Forge.Generators.Features.AutoPropertyGenerator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

namespace Forge.Tests.Common;

public static class AnalyzerTestHelper {
    public static ForgeAnalyzersTest<TAnalyzer, DefaultVerifier> CreateAnalyzerTest<TAnalyzer>(
        [StringSyntax("c#-test")] string inputSource)
            where TAnalyzer : DiagnosticAnalyzer, new()
    {
        ForgeAnalyzersTest<TAnalyzer, DefaultVerifier> test = new ForgeAnalyzersTest<TAnalyzer, DefaultVerifier> {
            TestBehaviors = TestBehaviors.SkipGeneratedCodeCheck,
            TestState = {
                Sources = { inputSource },
                ReferenceAssemblies = Utility.ReferenceAssemblies
            }
        };
        
        test.TestState.AdditionalReferences.AddRange(Utility.GetAdditionalReferences());
        
        return test;
    }
}


public class ForgeAnalyzersTest<TAnalyzer, TVerifier> : AnalyzerTest<TVerifier>
    where TAnalyzer : DiagnosticAnalyzer, new()
    where TVerifier : IVerifier, new()
{
    protected override CompilationOptions CreateCompilationOptions()
        => new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, allowUnsafe: true);
        
    protected override ParseOptions CreateParseOptions()
        => new CSharpParseOptions(LanguageVersion.Latest, DocumentationMode.Diagnose);
        
    protected override IEnumerable<DiagnosticAnalyzer> GetDiagnosticAnalyzers()
        => [new TAnalyzer()];

    // protected override IEnumerable<Type> GetSourceGenerators() 
    //     => [typeof(AutoPropertyGenerator)];

    protected override string DefaultFileExt => "cs";
    public override string Language => LanguageNames.CSharp;
}
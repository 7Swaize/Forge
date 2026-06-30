using System.Threading.Tasks;
using Forge.Analyzers.Features.NoLocalsAnalyzer;
using Forge.Tests.Common;
using Xunit;

namespace Forge.Tests.Features.NoLocalVariables.AnalyzerTests;

public sealed partial class Tests {
    [Fact]
    public async Task CannotContainLocalVariableDecl_WhenMethodLacksAttribute_DoesNotAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<NoLocalVariablesAnalyzer>(
            """
            class C {
                void Method() {
                    int x = 5;
                    string y = "hello";
                }
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);
    
    [Fact]
    public async Task CannotContainLocalVariableDecl_NoDeclaration_DoesNotAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<NoLocalVariablesAnalyzer>(
            """
            using Forge.Annotations;

            class C {
                [NoLocalVariables]
                void Method() { }
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);
    
    [Fact]
    public async Task CannotContainLocalVariableDecl_SingleLocalDeclaration_DoesAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<NoLocalVariablesAnalyzer>(
            """
            using Forge.Annotations;

            class C {
                [NoLocalVariables]
                void Method() {
                    {|FRG0200:int x = 5;|}
                }
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);
    

    [Fact]
    public async Task CannotContainLocalVariableDecl_MultipleLocalDeclarations_DoesAlertIndependently() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<NoLocalVariablesAnalyzer>(
            """
            using Forge.Annotations;

            class C {
                [NoLocalVariables]
                void Method() {
                    {|FRG0200:int    x = 1;|}
                    {|FRG0200:var    y = "hello";|}
                    {|FRG0200:bool   z = true;|}
                }
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);
    
    [Fact]
    public async Task CannotContainLocalVariableDecl_LocalNestedInsideControlFlow_DoesAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<NoLocalVariablesAnalyzer>(
            """
            using Forge.Annotations;
            
            class C {
                [NoLocalVariables]
                void Method(bool flag) {
                    if (flag) {
                        {|FRG0200:int x = 1;|}
                    }
            
                    for (int i = 0; i < 10; i++) {   // loop variable is NOT a LocalDeclarationStatement
                        {|FRG0200:string s = "loop";|}
                    }
                }
            }    
            """
        ).RunAsync(TestContext.Current.CancellationToken);
    
    [Fact]
    public async Task CannotContainLocalVariableDecl_WhenAnnotatedMethodIsExpressionBodied_DoesNotAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<NoLocalVariablesAnalyzer>(
            """
            using Forge.Annotations;
            
            class C {
                [NoLocalVariables]
                int Method(int a, int b) => a + b;
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);
    
    // The analyzer explicitly stops descending at LocalFunctionStatementSyntax, 
    // so locals inside the local function body must NOT trigger the diagnostic.
    [Fact]
    public async Task CannotContainLocalVariableDecl_ForLocalsInsideLocalFunction_DoesNotAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<NoLocalVariablesAnalyzer>(
            """
            using Forge.Annotations;
            
            class C {
                [NoLocalVariables]
                void Method() {
                    Inner();
                    
                    {|FRG0200:string s = "loop";|}      // Flagged
            
                    void Inner() {
                        int x = 5;      // Not flagged
                    }
                }
            }    
            """
        ).RunAsync(TestContext.Current.CancellationToken);
    
    // The analyzer stops descending at AnonymousFunctionExpressionSyntax, so
    // locals inside a lambda body must NOT trigger.
    [Fact]
    public async Task CannotContainLocalVariableDecl_ForLocalsInsideLambdaBody_DoesNotAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<NoLocalVariablesAnalyzer>(
            """
            using System;
            using Forge.Annotations;
            
            class C {
                private Action _callback;
            
                [NoLocalVariables]
                void Method() {
                    _callback = () => {
                        int x = 5;      // Not flagged
                    };
                }
            }   
            """
        ).RunAsync(TestContext.Current.CancellationToken);
    
    [Fact]
    public async Task CannotContainerLocalVariableDecl_ForLocalsInsideAnonymousDelegate_DoesNotAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<NoLocalVariablesAnalyzer>(
            """
            using System;
            using Forge.Annotations;
            
            class C {
                private Action _callback;   // field
            
                [NoLocalVariables]
                void Method() {
                    _callback = delegate {
                        string s = "anonymous";     // not flagged
                    };
                }
            }    
            """
        ).RunAsync(TestContext.Current.CancellationToken);
}
using System.Threading.Tasks;
using Forge.Analyzers.Features.CompileTimeConstantAnalyzer;
using Forge.Tests.Common;
using Xunit;

namespace Forge.Tests.Features.CompileTimeConstant.AnalyzerTests;

public sealed partial class Tests {
    [Fact]
    public async Task RequiresCompileTimeConstant_WhenParameterLacksAttribute_DoesNotAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<RequiresCompileTimeConstantDiagnosticAnalyzer>(
            """
            class C {
                static void Target(int value) { }

                static void Caller(int x) {
                    Target(x);
                }
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);
    
    [Fact]
    public async Task RequiresCompileTimeConstant_LiteralArgument_DoesNotAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<RequiresCompileTimeConstantDiagnosticAnalyzer>(
            """
            using Forge.Annotations;

            class C {
                static void Target([CompileTimeConstant] int value) { }

                static void Caller() {
                    Target(42);
                }
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);
    
    [Fact]
    public async Task RequiresCompileTimeConstant_ConstFieldAndConstLocal_DoesNotAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<RequiresCompileTimeConstantDiagnosticAnalyzer>(
            """
            using Forge.Annotations;

            class C {
                private const int Limit = 4;

                static void Target([CompileTimeConstant] int value) { }

                static void Caller() {
                    const int local = 2;

                    Target(Limit);
                    Target(local);
                    Target(int.MaxValue);
                }
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);
    
    [Fact]
    public async Task RequiresCompileTimeConstant_ConstantExpressions_DoesNotAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<RequiresCompileTimeConstantDiagnosticAnalyzer>(
            """
            using Forge.Annotations;

            class C {
                private const int Limit = 4;
                private const string Prefix = "id_";

                static void TargetInt([CompileTimeConstant] int value) { }
                static void TargetString([CompileTimeConstant] string value) { }

                static void Caller() {
                    TargetInt(1 + 2);
                    TargetInt(Limit * 2);
                    TargetInt(-Limit);
                    TargetString("a" + "b");
                    TargetString(Prefix + "42");
                    TargetString(nameof(Caller));
                }
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);
    
    [Fact]
    public async Task RequiresCompileTimeConstant_AllSupportedParameterTypes_DoesNotAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<RequiresCompileTimeConstantDiagnosticAnalyzer>(
            """
            using Forge.Annotations;

            enum Color { Red, Green }

            class C {
                static void TargetBool([CompileTimeConstant] bool value) { }
                static void TargetChar([CompileTimeConstant] char value) { }
                static void TargetLong([CompileTimeConstant] long value) { }
                static void TargetDouble([CompileTimeConstant] double value) { }
                static void TargetDecimal([CompileTimeConstant] decimal value) { }
                static void TargetString([CompileTimeConstant] string value) { }
                static void TargetEnum([CompileTimeConstant] Color value) { }

                static void Caller() {
                    TargetBool(true);
                    TargetChar('a');
                    TargetLong(1L);
                    TargetDouble(1.5);
                    TargetDecimal(1.5m);
                    TargetString("text");
                    TargetEnum(Color.Green);
                }
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);
    
    [Fact]
    public async Task RequiresCompileTimeConstant_OmittedOptionalArgument_DoesNotAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<RequiresCompileTimeConstantDiagnosticAnalyzer>(
            """
            using Forge.Annotations;

            class C {
                static void Target([CompileTimeConstant] int value = 5) { }

                static void Caller() {
                    Target();
                }
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);
    
    [Fact]
    public async Task RequiresCompileTimeConstant_NonConstantArgumentToUnannotatedParameter_DoesNotAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<RequiresCompileTimeConstantDiagnosticAnalyzer>(
            """
            using Forge.Annotations;

            class C {
                static void Target([CompileTimeConstant] int constant, int plain) { }

                static void Caller(int x) {
                    Target(1, x);       // only 'constant' is constrained
                }
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);
    
    [Fact]
    public async Task RequiresCompileTimeConstant_NonConstLocal_DoesAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<RequiresCompileTimeConstantDiagnosticAnalyzer>(
            """
            using Forge.Annotations;

            class C {
                static void Target([CompileTimeConstant] int value) { }

                static void Caller() {
                    int x = 2;  // not declared const
                    Target({|FRG0300:x|});
                }
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);
    
    [Fact]
    public async Task RequiresCompileTimeConstant_MethodParameter_DoesAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<RequiresCompileTimeConstantDiagnosticAnalyzer>(
            """
            using Forge.Annotations;

            class C {
                static void Target([CompileTimeConstant] int value) { }

                static void Caller(int x) {
                    Target({|FRG0300:x|});
                }
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);
    
    [Fact]
    public async Task RequiresCompileTimeConstant_NonConstFieldsAndProperties_DoesAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<RequiresCompileTimeConstantDiagnosticAnalyzer>(
            """
            using Forge.Annotations;

            class C {
                private static readonly int StaticReadonly = 1;
                private readonly int _instanceReadonly = 2;
                private int _field = 3;
                private int Prop => 4;

                static void Target([CompileTimeConstant] int value) { }

                void Caller() {
                    Target({|FRG0300:StaticReadonly|});
                    Target({|FRG0300:_instanceReadonly|});
                    Target({|FRG0300:_field|});
                    Target({|FRG0300:Prop|});
                }
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);
    
    [Fact]
    public async Task RequiresCompileTimeConstant_StringEmpty_DoesAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<RequiresCompileTimeConstantDiagnosticAnalyzer>(
            """
            using Forge.Annotations;

            class C {
                static void Target([CompileTimeConstant] string value) { }

                static void Caller() {
                    Target({|FRG0300:string.Empty|});
                    Target("");
                }
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);
    
    [Fact]
    public async Task RequiresCompileTimeConstant_MethodCallResult_DoesAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<RequiresCompileTimeConstantDiagnosticAnalyzer>(
            """
            using Forge.Annotations;

            class C {
                static int Compute(int a) => a;

                static void Target([CompileTimeConstant] int value) { }

                static void Caller() {
                    Target({|FRG0300:Compute(1)|});     // constant inputs do not make the result constant
                }
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);
    
    [Fact]
    public async Task RequiresCompileTimeConstant_ExpressionMixingConstantAndNonConstant_DoesAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<RequiresCompileTimeConstantDiagnosticAnalyzer>(
            """
            using Forge.Annotations;

            class C {
                private const int Limit = 4;

                static void TargetInt([CompileTimeConstant] int value) { }
                static void TargetString([CompileTimeConstant] string value) { }

                static void Caller(int x, string s) {
                    TargetInt({|FRG0300:x + 1|});
                    TargetInt({|FRG0300:Limit + x|});
                    TargetString({|FRG0300:"a" + s|});
                }
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);
    
    [Fact]
    public async Task RequiresCompileTimeConstant_NonConstEnumValue_DoesAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<RequiresCompileTimeConstantDiagnosticAnalyzer>(
            """
            using Forge.Annotations;

            enum Color { Red, Green }

            class C {
                static void Target([CompileTimeConstant] Color value) { }

                static void Caller(Color c) {
                    Target(Color.Red);
                    Target({|FRG0300:c|});
                }
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);
    
    [Fact]
    public async Task RequiresCompileTimeConstant_MultipleAnnotatedParameters_DoesAlertIndependently() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<RequiresCompileTimeConstantDiagnosticAnalyzer>(
            """
            using Forge.Annotations;

            class C {
                static void Target([CompileTimeConstant] int a, int plain, [CompileTimeConstant] int b) { }

                static void Caller(int x, int y) {
                    Target({|FRG0300:x|}, x, {|FRG0300:y|});    // 'plain' is not flagged
                    Target(1, x, {|FRG0300:y|});                // only the non-constant one is flagged
                    Target(1, x, 3);
                }
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);
    
    [Fact]
    public async Task RequiresCompileTimeConstant_NamedArguments_DoesAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<RequiresCompileTimeConstantDiagnosticAnalyzer>(
            """
            using Forge.Annotations;

            class C {
                static void Target(int plain, [CompileTimeConstant] int value) { }

                static void Caller(int x) {
                    Target(value: 1, plain: x);                 // constant goes to the annotated parameter
                    Target({|FRG0300:value: x|}, plain: 1);     // out of order, non-constant to annotated parameter
                }
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);
    
    [Fact]
    public async Task RequiresCompileTimeConstant_MultipleInvocations_DoesAlertIndependently() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<RequiresCompileTimeConstantDiagnosticAnalyzer>(
            """
            using Forge.Annotations;

            class C {
                static void Target([CompileTimeConstant] int value) { }

                static void Caller(int x) {
                    Target(1);
                    Target({|FRG0300:x|});
                    Target(2);
                    Target({|FRG0300:x|});
                }
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);
    
    [Fact]
    public async Task RequiresCompileTimeConstant_ExtensionMethodInvocation_DoesAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<RequiresCompileTimeConstantDiagnosticAnalyzer>(
            """
            using Forge.Annotations;

            static class Extensions {
                public static void Twice(this string self, [CompileTimeConstant] int count) { }
            }

            class C {
                void Caller(int x) {
                    "a".Twice(2);
                    "a".Twice({|FRG0300:x|});
                }
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);

    [Fact]
    public async Task RequiresCompileTimeConstant_MethodDeclaredInAnotherType_DoesAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<RequiresCompileTimeConstantDiagnosticAnalyzer>(
            """
            using Forge.Annotations;

            static class Api {
                public static void Target([CompileTimeConstant] int value) { }
            }

            class C {
                void Caller(int x) {
                    Api.Target(1);
                    Api.Target({|FRG0300:x|});
                }
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);
    
        [Theory]
    [InlineData("object")]
    [InlineData("System.DateTime")]
    [InlineData("System.TimeSpan")]
    [InlineData("int[]")]
    [InlineData("int?")]
    [InlineData("Custom")]
    [InlineData("CustomStruct")]
    public async Task AttributeOnInvalidTargetType_UnsupportedType_DoesAlert(string type) =>
        await AnalyzerTestHelper.CreateAnalyzerTest<AttributeOnInvalidTargetTypeDiagnosticAnalyzer>(
            $$"""
            using Forge.Annotations;

            class Custom { }
            struct CustomStruct { }

            class C {
                void Method({|FRG0301:[CompileTimeConstant] {{type}} value|}) { }
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);

    [Fact]
    public async Task AttributeOnInvalidTargetType_WithDefaultValue_DoesAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<AttributeOnInvalidTargetTypeDiagnosticAnalyzer>(
            """
            using Forge.Annotations;

            class C {
                void Method({|FRG0301:[CompileTimeConstant] object value = null|}) { }
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);

    [Fact]
    public async Task AttributeOnInvalidTargetType_MultipleParameters_DoesAlertIndependently() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<AttributeOnInvalidTargetTypeDiagnosticAnalyzer>(
            """
            using Forge.Annotations;

            class C {
                void Method(
                    [CompileTimeConstant] int valid,
                    {|FRG0301:[CompileTimeConstant] object invalid|},
                    object notAnnotated,
                    {|FRG0301:[CompileTimeConstant] System.DateTime alsoInvalid|}) { }
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);

    [Fact]
    public async Task AttributeOnInvalidTargetType_StaticMethodInGenericType_DoesAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<AttributeOnInvalidTargetTypeDiagnosticAnalyzer>(
            """
            using Forge.Annotations;

            static class Api<T> {
                public static void Method([CompileTimeConstant] int valid, {|FRG0301:[CompileTimeConstant] T invalid|}) { }
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);
}
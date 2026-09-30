using System.Threading.Tasks;
using Forge.Analyzers.Features.CompileTimeConstantAnalyzer;
using Forge.Tests.Common;
using Xunit;

namespace Forge.Tests.Features.CompileTimeConstant.AnalyzerTests;

public sealed partial class Tests {
    [Theory]
    [InlineData("bool")]
    [InlineData("char")]
    [InlineData("sbyte")]
    [InlineData("byte")]
    [InlineData("short")]
    [InlineData("ushort")]
    [InlineData("int")]
    [InlineData("uint")]
    [InlineData("long")]
    [InlineData("ulong")]
    [InlineData("float")]
    [InlineData("double")]
    [InlineData("decimal")]
    [InlineData("string")]
    public async Task AttributeOnInvalidTargetType_SupportedType_DoesNotAlert(string type) =>
        await AnalyzerTestHelper.CreateAnalyzerTest<AttributeOnInvalidTargetTypeDiagnosticAnalyzer>(
            $$"""
              using Forge.Annotations;

              class C {
                  void Method([CompileTimeConstant] {{type}} value) { }
              }
              """
        ).RunAsync(TestContext.Current.CancellationToken);

    [Fact]
    public async Task AttributeOnInvalidTargetType_Enum_DoesNotAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<AttributeOnInvalidTargetTypeDiagnosticAnalyzer>(
            """
            using Forge.Annotations;

            enum Color { Red, Green }

            class C {
                void Method([CompileTimeConstant] Color value) { }
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);

    [Fact]
    public async Task AttributeOnInvalidTargetType_UnsupportedTypeWithoutAttribute_DoesNotAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<AttributeOnInvalidTargetTypeDiagnosticAnalyzer>(
            """
            class C {
                void Method(object a, System.DateTime b, int[] c) { }
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);
}
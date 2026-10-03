using System.Threading.Tasks;
using Forge.Analyzers.Features.RemoveByAnalyzers;
using Forge.Tests.Common;
using Xunit;

namespace Forge.Tests.Features.RemoveBy.AnalyzerTests;

public sealed partial class Tests {
    [Theory]
    [InlineData("tomorrow")]
    [InlineData("")]
    [InlineData("2025/01/05")]
    [InlineData("01-05-2025")]
    [InlineData("2025-1-5")]
    [InlineData("2025-02-30")]
    [InlineData("2025-13-01")]
    [InlineData("2025-01-05T00:00:00")]
    [InlineData(" 2000-01-01")]
    public async Task InvalidDateTimeFormat_MalformedDate_DoesAlert(string date) =>
        await AnalyzerTestHelper.CreateAnalyzerTest<RemoveByAttributeAnalyzer>(
            $$"""
            using Forge.Annotations;

            class C {
                [{|FRG0502:RemoveBy("{{date}}")|}]
                void Method() { }
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);

    [Fact]
    public async Task InvalidDateTimeFormat_WithReason_DoesAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<RemoveByAttributeAnalyzer>(
            """
            using Forge.Annotations;

            class C {
                [{|FRG0502:RemoveBy("soon", "temporary workaround")|}]
                void Method() { }
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);

    [Fact]
    public async Task InvalidDateTimeFormat_OnTypesAndMembers_DoesAlertIndependently() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<RemoveByAttributeAnalyzer>(
            """
            using Forge.Annotations;

            [{|FRG0502:RemoveBy("bad")|}]
            class C {
                [{|FRG0502:RemoveBy("bad")|}] int _field;
                [{|FRG0502:RemoveBy("bad")|}] int Prop { get; set; }
                [RemoveBy("2000-01-01")] void {|FRG0501:Method|}() { }
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);
}
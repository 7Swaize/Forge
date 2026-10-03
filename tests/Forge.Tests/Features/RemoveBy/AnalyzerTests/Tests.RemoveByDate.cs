using System.Threading.Tasks;
using Forge.Analyzers.Features.RemoveByAnalyzers;
using Forge.Tests.Common;
using Xunit;

namespace Forge.Tests.Features.RemoveBy.AnalyzerTests;


public sealed partial class Tests {
    [Fact]
    public async Task RemoveBy_WhenSymbolLacksAttribute_DoesNotAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<RemoveByAttributeAnalyzer>(
            """
            class C {
                int _field;
                int Prop { get; set; }
                void Method() { }
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);

    [Fact]
    public async Task RemoveBy_UnrelatedAttributeWithDateString_DoesNotAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<RemoveByAttributeAnalyzer>(
            """
            class C {
                [System.Obsolete("2000-01-01")]
                void Method() { }
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);

    [Fact]
    public async Task RemoveBy_LookalikeAttributeFromAnotherNamespace_DoesNotAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<RemoveByAttributeAnalyzer>(
            """
            class RemoveByAttribute : System.Attribute {
                public RemoveByAttribute(string date) { }
            }

            class C {
                [RemoveBy("2000-01-01")]
                void Method() { }
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);

    [Fact]
    public async Task RemoveBy_FutureDate_DoesAlertWithWarning() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<RemoveByAttributeAnalyzer>(
            """
            using Forge.Annotations;

            class C {
                [RemoveBy("2999-12-31")]
                void {|FRG0500:Method|}() { }
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);

    [Fact]
    public async Task RemoveBy_PastDate_DoesAlertWithError() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<RemoveByAttributeAnalyzer>(
            """
            using Forge.Annotations;

            class C {
                [RemoveBy("2000-01-01")]
                void {|FRG0501:Method|}() { }
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);

    [Fact]
    public async Task RemoveBy_WithReason_DoesAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<RemoveByAttributeAnalyzer>(
            """
            using Forge.Annotations;

            class C {
                [RemoveBy("2000-01-01", "temporary workaround")]
                void {|FRG0501:Past|}() { }

                [RemoveBy("2999-12-31", reason: "temporary workaround")]
                void {|FRG0500:Future|}() { }
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);

    [Fact]
    public async Task RemoveBy_MultipleAnnotatedMembers_DoesAlertIndependently() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<RemoveByAttributeAnalyzer>(
            """
            using Forge.Annotations;

            class C {
                [RemoveBy("2000-01-01")]
                void {|FRG0501:Expired|}() { }

                [RemoveBy("2999-12-31")]
                void {|FRG0500:Scheduled|}() { }

                void NotAnnotated() { }

                [{|FRG0502:RemoveBy("not-a-date")|}]
                void Broken() { }
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);
    
    [Fact]
    public async Task RemoveBy_ValidTargets_FutureDate_DoesAlertWithWarning() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<RemoveByAttributeAnalyzer>(
            """
            using Forge.Annotations;

            [RemoveBy("2999-12-31")] class {|FRG0500:AClass|} {
                [RemoveBy("2999-12-31")] public {|FRG0500:AClass|}() { }
                [RemoveBy("2999-12-31")] int {|FRG0500:_field|};
                [RemoveBy("2999-12-31")] int {|FRG0500:Prop|} { get; set; }
                [RemoveBy("2999-12-31")] void {|FRG0500:Method|}() { }
            }

            [RemoveBy("2999-12-31")] struct {|FRG0500:AStruct|} { }
            [RemoveBy("2999-12-31")] enum {|FRG0500:AnEnum|} { A }
            [RemoveBy("2999-12-31")] interface {|FRG0500:IThing|} { }
            """
        ).RunAsync(TestContext.Current.CancellationToken);

    [Fact]
    public async Task RemoveBy_ValidTargets_PastDate_DoesAlertWithError() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<RemoveByAttributeAnalyzer>(
            """
            using Forge.Annotations;

            [RemoveBy("2000-01-01")] class {|FRG0501:AClass|} {
                [RemoveBy("2000-01-01")] public {|FRG0501:AClass|}() { }
                [RemoveBy("2000-01-01")] int {|FRG0501:_field|};
                [RemoveBy("2000-01-01")] int {|FRG0501:Prop|} { get; set; }
                [RemoveBy("2000-01-01")] void {|FRG0501:Method|}() { }
            }

            [RemoveBy("2000-01-01")] struct {|FRG0501:AStruct|} { }
            [RemoveBy("2000-01-01")] enum {|FRG0501:AnEnum|} { A }
            [RemoveBy("2000-01-01")] interface {|FRG0501:IThing|} { }
            """
        ).RunAsync(TestContext.Current.CancellationToken);
    
    [Fact]
    public async Task RemoveBy_MultipleFieldDeclarationsInOneLine_DoesAlertForEach() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<RemoveByAttributeAnalyzer>(
            """
            using Forge.Annotations;

            class C {
                [RemoveBy("2999-12-31")]
                int {|FRG0500:F1|}, {|FRG0500:F2|}, {|FRG0500:F3|};

                [RemoveBy("2000-01-01")]
                string {|FRG0501:F4|} = "", {|FRG0501:F5|} = "";
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);

    [Fact]
    public async Task RemoveBy_AnnotatedTypeAndAnnotatedMember_DoesAlertIndependently() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<RemoveByAttributeAnalyzer>(
            """
            using Forge.Annotations;

            [RemoveBy("2999-12-31")]
            class {|FRG0500:Outer|} {
                [RemoveBy("2000-01-01")]
                void {|FRG0501:Inner|}() { }

                void Plain() { }
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);

    [Fact]
    public async Task RemoveBy_GenericType_DoesAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<RemoveByAttributeAnalyzer>(
            """
            using Forge.Annotations;

            [RemoveBy("2000-01-01")]
            class {|FRG0501:Box|}<T> { }
            """
        ).RunAsync(TestContext.Current.CancellationToken);
}
using System.Threading.Tasks;
using Forge.Analyzers.Features.BlittableAnalyzers;
using Forge.Analyzers.Features.BlittableAnalyzers.Diagnostics;
using Forge.Tests.Common;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace Forge.Tests.Features.Blittable.AnalyzerTests;

public sealed partial class Tests {
    [Fact]
    public async Task IncompatibleLayout_Sequential_DoesNotAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<BlittableAttributeAnalyzer>(
            """
            using System.Runtime.InteropServices;
            using Forge.Annotations;

            [Blittable]
            [StructLayout(LayoutKind.Sequential)]
            public struct Foo {
                public int Id;
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);

    [Fact]
    public async Task IncompatibleLayout_Explicit_DoesNotAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<BlittableAttributeAnalyzer>(
            """
            using System.Runtime.InteropServices;
            using Forge.Annotations;

            [Blittable]
            [StructLayout(LayoutKind.Explicit)]
            public struct Foo {
                [FieldOffset(0)] public int Id;
                [FieldOffset(4)] public float Value;
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);

    [Fact]
    public async Task IncompatibleLayout_NoStructLayoutAttribute_DoesNotAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<BlittableAttributeAnalyzer>(
            """
            using Forge.Annotations;

            [Blittable]
            public struct Foo {
                public int Id;
                public float Value;
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);

    [Fact]
    public async Task IncompatibleLayout_Sequential_WithNamedArguments_DoesNotAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<BlittableAttributeAnalyzer>(
            """
            using System.Runtime.InteropServices;
            using Forge.Annotations;

            [Blittable]
            [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
            public struct Foo {
                public int Id;
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);

    [Fact]
    public async Task IncompatibleLayout_Auto_DoesAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<BlittableAttributeAnalyzer>(
            """
            using System.Runtime.InteropServices;
            using Forge.Annotations;

            [Blittable]
            [StructLayout(LayoutKind.Auto)]
            public struct {|FRG0601:Foo|} {
                public int Id;
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);

    [Fact]
    public async Task IncompatibleLayout_Auto_WithNamedArguments_DoesAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<BlittableAttributeAnalyzer>(
            """
            using System.Runtime.InteropServices;
            using Forge.Annotations;

            [Blittable]
            [StructLayout(LayoutKind.Auto, Pack = 1, CharSet = CharSet.Unicode)]
            public struct {|FRG0601:Foo|} {
                public int Id;
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);

    [Fact]
    public async Task IncompatibleLayout_Auto_ShortOverload_DoesAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<BlittableAttributeAnalyzer>(
            """
            using System.Runtime.InteropServices;
            using Forge.Annotations;

            [Blittable]
            [StructLayout((short)3)]
            public struct {|FRG0601:Foo|} {
                public int Id;
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);

    [Fact]
    public async Task IncompatibleLayout_Sequential_ShortOverload_DoesNotAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<BlittableAttributeAnalyzer>(
            """
            using System.Runtime.InteropServices;
            using Forge.Annotations;

            [Blittable]
            [StructLayout((short)0)]
            public struct Foo {
                public int Id;
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);

    [Fact]
    public async Task IncompatibleLayout_Auto_LongFormAttributeName_DoesAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<BlittableAttributeAnalyzer>(
            """
            using System.Runtime.InteropServices;
            using Forge.Annotations;

            [BlittableAttribute]
            [StructLayout(LayoutKind.Auto)]
            public struct {|FRG0601:Foo|} {
                public int Id;
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);

    [Fact]
    public async Task IncompatibleLayout_Auto_WithoutBlittableAttribute_DoesNotAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<BlittableAttributeAnalyzer>(
            """
            using System.Runtime.InteropServices;

            [StructLayout(LayoutKind.Auto)]
            public struct Foo {
                public int Id;
                public string Name;
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);

    [Fact]
    public async Task IncompatibleLayout_Auto_NestedInsideClass_DoesAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<BlittableAttributeAnalyzer>(
            """
            using System.Runtime.InteropServices;
            using Forge.Annotations;

            public class Host {
                [Blittable]
                [StructLayout(LayoutKind.Auto)]
                public struct {|FRG0601:Foo|} {
                    public int Id;
                }
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);

    [Fact]
    public async Task IncompatibleLayout_Auto_WithNonBlittableField_ReportsBothDiagnostics() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<BlittableAttributeAnalyzer>(
            """
            using System.Runtime.InteropServices;
            using Forge.Annotations;

            [Blittable]
            [StructLayout(LayoutKind.Auto)]
            public struct {|FRG0601:Foo|} {
                public int Id;
                public string {|FRG0600:Name|};
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);

    [Fact]
    public async Task IncompatibleLayout_Auto_ReportsStructNameInMessage() {
        var test = AnalyzerTestHelper.CreateAnalyzerTest<BlittableAttributeAnalyzer>(
            """
            using System.Runtime.InteropServices;
            using Forge.Annotations;

            [Blittable]
            [StructLayout(LayoutKind.Auto)]
            public struct {|#0:Foo|} {
                public int Id;
            }
            """
        );

        test.ExpectedDiagnostics.Add(
            new DiagnosticResult(IncompatibleLayoutDiagnostic.CreateDescriptor())
                .WithLocation(0)
                .WithArguments("Foo")
        );

        await test.RunAsync(TestContext.Current.CancellationToken);
    }
}
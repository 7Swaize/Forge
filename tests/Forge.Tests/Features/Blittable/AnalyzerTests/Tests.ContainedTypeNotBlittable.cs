using System.Threading.Tasks;
using Forge.Analyzers.Features.BlittableAnalyzers;
using Forge.Analyzers.Features.BlittableAnalyzers.Diagnostics;
using Forge.Tests.Common;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace Forge.Tests.Features.Blittable.AnalyzerTests;

public sealed partial class Tests {
    [Theory]
    [InlineData("byte")]
    [InlineData("sbyte")]
    [InlineData("short")]
    [InlineData("ushort")]
    [InlineData("int")]
    [InlineData("uint")]
    [InlineData("long")]
    [InlineData("ulong")]
    [InlineData("float")]
    [InlineData("double")]
    [InlineData("nint")]
    [InlineData("nuint")]
    [InlineData("System.IntPtr")]
    [InlineData("System.UIntPtr")]
    public async Task ContainedTypeNotBlittable_BlittablePrimitive_DoesNotAlert(string type) =>
        await AnalyzerTestHelper.CreateAnalyzerTest<BlittableAttributeAnalyzer>(
            $$"""
              using System.Runtime.InteropServices;
              using Forge.Annotations;

              [Blittable]
              [StructLayout(LayoutKind.Sequential)]
              public struct Foo {
                  public {{type}} Value;
              }
              """
        ).RunAsync(TestContext.Current.CancellationToken);

    [Theory]
    [InlineData("bool")]
    [InlineData("char")]
    [InlineData("decimal")]
    [InlineData("string")]
    [InlineData("object")]
    [InlineData("int[]")]
    [InlineData("int?")]
    [InlineData("System.Action")]
    [InlineData("System.IDisposable")]
    [InlineData("System.Collections.Generic.List<int>")]
    public async Task ContainedTypeNotBlittable_NonBlittableType_DoesAlert(string type) =>
        await AnalyzerTestHelper.CreateAnalyzerTest<BlittableAttributeAnalyzer>(
            $$"""
              using System.Runtime.InteropServices;
              using Forge.Annotations;

              [Blittable]
              [StructLayout(LayoutKind.Sequential)]
              public struct Foo {
                  public int Id;
                  public {{type}} {|FRG0600:Value|};
              }
              """
        ).RunAsync(TestContext.Current.CancellationToken);

    [Fact]
    public async Task ContainedTypeNotBlittable_UserDefinedClass_DoesAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<BlittableAttributeAnalyzer>(
            """
            using System.Runtime.InteropServices;
            using Forge.Annotations;

            public class Node { }

            [Blittable]
            [StructLayout(LayoutKind.Sequential)]
            public struct Foo {
                public Node {|FRG0600:Next|};
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);

    [Fact]
    public async Task ContainedTypeNotBlittable_Enum_DoesNotAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<BlittableAttributeAnalyzer>(
            """
            using System;
            using System.Runtime.InteropServices;
            using Forge.Annotations;

            public enum Mode : byte { A, B }

            [Blittable]
            [StructLayout(LayoutKind.Sequential)]
            public struct Foo {
                public Mode Mode;
                public DayOfWeek Day;
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);

    [Fact]
    public async Task ContainedTypeNotBlittable_Pointers_DoesNotAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<BlittableAttributeAnalyzer>(
            """
            using System.Runtime.InteropServices;
            using Forge.Annotations;

            [Blittable]
            [StructLayout(LayoutKind.Sequential)]
            public unsafe struct Foo {
                public int* Typed;
                public void* Untyped;
                public delegate*<int, void> Callback;
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);

    [Fact]
    public async Task ContainedTypeNotBlittable_FixedSizeBuffer_DoesNotAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<BlittableAttributeAnalyzer>(
            """
            using System.Runtime.InteropServices;
            using Forge.Annotations;

            [Blittable]
            [StructLayout(LayoutKind.Sequential)]
            public unsafe struct Foo {
                public int Id;
                public fixed int Values[10];
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);

    [Fact]
    public async Task ContainedTypeNotBlittable_DeeplyNestedBlittableStructs_DoesNotAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<BlittableAttributeAnalyzer>(
            """
            using System.Runtime.InteropServices;
            using Forge.Annotations;

            public struct Level3 { public long Value; }
            public struct Level2 { public Level3 Next; public int Extra; }
            public struct Level1 { public Level2 Next; }

            [Blittable]
            [StructLayout(LayoutKind.Sequential)]
            public struct Foo {
                public Level1 Value;
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);

    [Fact]
    public async Task ContainedTypeNotBlittable_NestedStructWithFixedBuffer_DoesNotAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<BlittableAttributeAnalyzer>(
            """
            using System.Runtime.InteropServices;
            using Forge.Annotations;

            public unsafe struct Inner {
                public fixed byte Data[4];
            }

            [Blittable]
            [StructLayout(LayoutKind.Sequential)]
            public struct Foo {
                public Inner Value;
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);

    [Fact]
    public async Task ContainedTypeNotBlittable_NestedFrameworkStruct_DoesNotAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<BlittableAttributeAnalyzer>(
            """
            using System.Numerics;
            using System.Runtime.InteropServices;
            using Forge.Annotations;

            [Blittable]
            [StructLayout(LayoutKind.Sequential)]
            public struct Foo {
                public Vector3 Position;
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);

    [Fact]
    public async Task ContainedTypeNotBlittable_NestedStructWithReferenceField_DoesAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<BlittableAttributeAnalyzer>(
            """
            using System.Runtime.InteropServices;
            using Forge.Annotations;

            public struct Inner {
                public string Name;
            }

            [Blittable]
            [StructLayout(LayoutKind.Sequential)]
            public struct Foo {
                public Inner {|FRG0600:Value|};
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);
    
    [Theory]
    [InlineData("bool")]
    [InlineData("char")]
    [InlineData("decimal")]
    public async Task ContainedTypeNotBlittable_NestedStructWithNonBlittablePrimitive_DoesAlert(string type) =>
        await AnalyzerTestHelper.CreateAnalyzerTest<BlittableAttributeAnalyzer>(
            $$"""
              using System.Runtime.InteropServices;
              using Forge.Annotations;

              public struct Inner {
                  public {{type}} Value;
              }

              [Blittable]
              [StructLayout(LayoutKind.Sequential)]
              public struct Foo {
                  public Inner {|FRG0600:Value|};
              }
              """
        ).RunAsync(TestContext.Current.CancellationToken);
    
    [Fact]
    public async Task ContainedTypeNotBlittable_OnlyOffendingFieldsAreReported() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<BlittableAttributeAnalyzer>(
            """
            using System.Runtime.InteropServices;
            using Forge.Annotations;

            [Blittable]
            [StructLayout(LayoutKind.Sequential)]
            public struct Foo {
                public int Id;
                public string {|FRG0600:Name|};
                public float Weight;
                public object {|FRG0600:Tag|};
                public bool {|FRG0600:Enabled|};
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);

    [Fact]
    public async Task ContainedTypeNotBlittable_MultipleDeclarators_EachIsReported() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<BlittableAttributeAnalyzer>(
            """
            using System.Runtime.InteropServices;
            using Forge.Annotations;

            [Blittable]
            [StructLayout(LayoutKind.Sequential)]
            public struct Foo {
                public string {|FRG0600:A|}, {|FRG0600:B|};
                public int C, D;
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);

    [Fact]
    public async Task ContainedTypeNotBlittable_StaticField_DoesNotAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<BlittableAttributeAnalyzer>(
            """
            using System.Runtime.InteropServices;
            using Forge.Annotations;

            [Blittable]
            [StructLayout(LayoutKind.Sequential)]
            public struct Foo {
                public int Id;
                public static string Name = "unnamed";
                public static object Shared = new object();
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);

    [Fact]
    public async Task ContainedTypeNotBlittable_ConstField_DoesNotAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<BlittableAttributeAnalyzer>(
            """
            using System.Runtime.InteropServices;
            using Forge.Annotations;

            [Blittable]
            [StructLayout(LayoutKind.Sequential)]
            public struct Foo {
                public const string Tag = "foo";
                public const bool Flag = true;
                public int Id;
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);

    [Fact]
    public async Task ContainedTypeNotBlittable_BlittableAutoProperty_DoesNotAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<BlittableAttributeAnalyzer>(
            """
            using System.Runtime.InteropServices;
            using Forge.Annotations;

            [Blittable]
            [StructLayout(LayoutKind.Sequential)]
            public struct Foo {
                public int Id { get; set; }
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);

    [Fact]
    public async Task ContainedTypeNotBlittable_NonBlittableAutoProperty_DoesAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<BlittableAttributeAnalyzer>(
            """
            using System.Runtime.InteropServices;
            using Forge.Annotations;

            [Blittable]
            [StructLayout(LayoutKind.Sequential)]
            public struct Foo {
                public string {|FRG0600:Name|} { get; set; }
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);

    [Fact]
    public async Task ContainedTypeNotBlittable_NestedTypeDeclaration_IsNotCountedAgainstOuterStruct() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<BlittableAttributeAnalyzer>(
            """
            using System.Runtime.InteropServices;
            using Forge.Annotations;

            [Blittable]
            [StructLayout(LayoutKind.Sequential)]
            public struct Foo {
                public int Id;

                public struct Unrelated {
                    public string Name;
                }
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);

    [Fact]
    public async Task ContainedTypeNotBlittable_EmptyStruct_DoesNotAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<BlittableAttributeAnalyzer>(
            """
            using System.Runtime.InteropServices;
            using Forge.Annotations;

            [Blittable]
            [StructLayout(LayoutKind.Sequential)]
            public struct Foo { }
            """
        ).RunAsync(TestContext.Current.CancellationToken);

    [Fact]
    public async Task ContainedTypeNotBlittable_GenericTypeParameter_DoesAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<BlittableAttributeAnalyzer>(
            """
            using System.Runtime.InteropServices;
            using Forge.Annotations;

            [Blittable]
            [StructLayout(LayoutKind.Sequential)]
            public struct Foo<T> where T : unmanaged {
                public T {|FRG0600:Value|};
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);
    
    [Fact]
    public async Task ContainedTypeNotBlittable_WithoutBlittableAttribute_DoesNotAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<BlittableAttributeAnalyzer>(
            """
            using System.Runtime.InteropServices;

            [StructLayout(LayoutKind.Sequential)]
            public struct Foo {
                public string Name;
                public object Tag;
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);

    [Fact]
    public async Task ContainedTypeNotBlittable_NoStructLayoutAttribute_BlittableFields_DoesNotAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<BlittableAttributeAnalyzer>(
            """
            using Forge.Annotations;

            [Blittable]
            public struct Foo {
                public int Id;
                public float Weight;
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);

    [Fact]
    public async Task ContainedTypeNotBlittable_NoStructLayoutAttribute_NonBlittableField_DoesAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<BlittableAttributeAnalyzer>(
            """
            using Forge.Annotations;

            [Blittable]
            public struct Foo {
                public int Id;
                public string {|FRG0600:Name|};
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);
    
    [Fact]
    public async Task ContainedTypeNotBlittable_ReportsOffendingTypeInMessage() {
        var test = AnalyzerTestHelper.CreateAnalyzerTest<BlittableAttributeAnalyzer>(
            """
            using System.Runtime.InteropServices;
            using Forge.Annotations;

            [Blittable]
            [StructLayout(LayoutKind.Sequential)]
            public struct Foo {
                public int Id;
                public string {|#0:Name|};
            }
            """
        );

        test.ExpectedDiagnostics.Add(
            new DiagnosticResult(ContainedTypeNotBlittableDiagnostic.CreateDescriptor())
                .WithLocation(0)
                .WithArguments("string")
        );

        await test.RunAsync(TestContext.Current.CancellationToken);
    }
}
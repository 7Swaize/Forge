using System.Threading.Tasks;
using Forge.Analyzers.Features.AutoPropertyAnalyzer;
using Forge.Tests.Common;
using Xunit;

namespace Forge.Tests.Features.AutoProperty.AnalyzerTests;

public sealed partial class Tests {
    [Fact]
    public async Task ReturnModeOnUnsupportedAccessor_Get_Default_DoesNotAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<AutoPropertyAnalyzer>(
            """
            using Forge.Annotations;

            [assembly: AutoPropertyNamingPolicy(NamingPolicy.PascalCase)]

            public partial class Foo {
                [AutoProperty(Visibility.Public, Accessors.Get, ReturnMode.Default)]
                private int _value;
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);
    
    [Fact]
    public async Task ReturnModeOnUnsupportedAccessor_Get_RefStruct_DoesNotAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<AutoPropertyAnalyzer>(
            """
            using Forge.Annotations;

            [assembly: AutoPropertyNamingPolicy(NamingPolicy.PascalCase)]

            public partial class Foo {
                [AutoProperty(Visibility.Public, Accessors.Get, ReturnMode.Default)]
                private int _value;
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);
    
    [Fact]
    public async Task ReturnModeOnUnsupportedAccessor_Get_RefReadonlyStruct_DoesNotAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<AutoPropertyAnalyzer>(
            """
            using Forge.Annotations;

            [assembly: AutoPropertyNamingPolicy(NamingPolicy.PascalCase)]

            public partial class Foo {
                [AutoProperty(Visibility.Public, Accessors.Get, ReturnMode.Default)]
                private int _value;
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);
    
    [Fact]
    public async Task ReturnModeOnUnsupportedAccessor_Set_RefStruct_DoesAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<AutoPropertyAnalyzer>(
            """
            using Forge.Annotations;

            [assembly: AutoPropertyNamingPolicy(NamingPolicy.PascalCase)]

            public partial class Foo {
                [AutoProperty(Visibility.Public, Accessors.Set, ReturnMode.RefStruct)]
                private int {|FRG0100:_value|};
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);
    
    [Fact]
    public async Task ReturnModeOnUnsupportedAccessor_Set_RefReadonlyStruct_DoesAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<AutoPropertyAnalyzer>(
            """
            using Forge.Annotations;

            [assembly: AutoPropertyNamingPolicy(NamingPolicy.PascalCase)]

            public partial class Foo {
                [AutoProperty(Visibility.Public, Accessors.Set, ReturnMode.RefReadonlyStruct)]
                private int {|FRG0100:_value|};
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);
    
    [Fact]
    public async Task ReturnModeOnUnsupportedAccessor_GetAndSet_RefStruct_DoesAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<AutoPropertyAnalyzer>(
            """
            using Forge.Annotations;

            [assembly: AutoPropertyNamingPolicy(NamingPolicy.PascalCase)]

            public partial class Foo {
                [AutoProperty(Visibility.Public, Accessors.GetAndSet, ReturnMode.RefStruct)]
                private int {|FRG0100:_value|};
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);
    
    [Fact]
    public async Task ReturnModeOnUnsupportedAccessor_GetAndSet_RefReadonlyStruct_DoesAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<AutoPropertyAnalyzer>(
            """
            using Forge.Annotations;

            [assembly: AutoPropertyNamingPolicy(NamingPolicy.PascalCase)]

            public partial class Foo {
                [AutoProperty(Visibility.Public, Accessors.GetAndSet, ReturnMode.RefReadonlyStruct)]
                private int {|FRG0100:_value|};
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);
    
    [Fact]
    public async Task ReturnModeOnUnsupportedAccessor_GetAndInit_RefStruct_DoesAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<AutoPropertyAnalyzer>(
            """
            using Forge.Annotations;

            [assembly: AutoPropertyNamingPolicy(NamingPolicy.PascalCase)]

            public partial class Foo {
                [AutoProperty(Visibility.Public, Accessors.GetAndInit, ReturnMode.RefStruct)]
                private int {|FRG0100:_value|};
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);
    
    [Fact]
    public async Task ReturnModeOnUnsupportedAccessor_GetAndInit_RefReadonlyStruct_DoesAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<AutoPropertyAnalyzer>(
            """
            using Forge.Annotations;

            [assembly: AutoPropertyNamingPolicy(NamingPolicy.PascalCase)]

            public partial class Foo {
                [AutoProperty(Visibility.Public, Accessors.GetAndInit, ReturnMode.RefReadonlyStruct)]
                private int {|FRG0100:_value|};
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);
}
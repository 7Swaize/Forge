using System.Threading.Tasks;
using Forge.Analyzers.Features.AutoPropertyAnalyzer;
using Forge.Tests.Common;
using Xunit;

namespace Forge.Tests.Features.AutoProperty.AnalyzerTests;

public sealed partial class Tests {
    [Fact]
    public async Task ContainingTypeMustBePartial_DoesNotAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<AutoPropertyAnalyzer>(
            """
            using Forge.Annotations;
            
            [assembly: AutoPropertyNamingPolicy(NamingPolicy.PascalCase)]
            
            public partial class Foo {
                [AutoProperty(Visibility.Public, Accessors.Get)]
                private int _value;
            }
            """  
        ).RunAsync(TestContext.Current.CancellationToken);
    
    [Fact]
    public async Task ContainingTypeMustBePartial_DoesAlert() =>
        await AnalyzerTestHelper.CreateAnalyzerTest<AutoPropertyAnalyzer>(
            """
            using Forge.Annotations;

            [assembly: AutoPropertyNamingPolicy(NamingPolicy.PascalCase)]

            public class {|FRG0001:Foo|} {
                [AutoProperty(Visibility.Public, Accessors.Get)]
                private int _value;
            }
            """
        ).RunAsync(TestContext.Current.CancellationToken);
    
}
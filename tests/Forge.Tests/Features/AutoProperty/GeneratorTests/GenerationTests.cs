using System;
using System.Threading.Tasks;
using Forge.Annotations;
using Forge.Generators.Features.AutoPropertyGenerator;
using Forge.Tests.Common;
using Microsoft.CodeAnalysis;
using VerifyTests;
using Xunit;
using static VerifyXunit.Verifier;

namespace Forge.Tests.Features.AutoProperty.GeneratorTests;

public sealed class GenerationTests {
    public static TheoryData<NamingPolicy> AllNamingPolicies => new(Enum.GetValues<NamingPolicy>());
    
    [Theory]
    [ClassData<AutoPropertyPermutations>]
    internal async Task AutoPropertyGenerator_EmitsAutoPropertyAllPermutationsNoArityContainer(
        Visibility visibility,
        Accessors accessors,
        ReturnMode returnMode)
    {
        GeneratorDriverRunResult result = GeneratorTestHelper.RunGenerator<AutoPropertyGenerator>(
            $$"""
               using System;
               using System.Collections.Generic;
               using Forge.Annotations;
               
               [assembly: AutoPropertyNamingPolicy(NamingPolicy.PascalCase)]

               public partial class Foo {
                   [AutoProperty(Visibility.{{visibility}}, Accessors.{{accessors}}, ReturnMode.{{returnMode}})]
                   private int _value;
                   
                   [AutoProperty(Visibility.{{visibility}}, Accessors.{{accessors}}, ReturnMode.{{returnMode}})]
                   private Dictionary<string, Lazy<int[]>> _dictionary;
               }
               """
        );

        _ = await Verify(result)
            .HashParameters()
            .UseParameters(visibility, accessors, returnMode);
    }

    [Theory]
    [ClassData<AutoPropertyPermutations>]
    internal async Task AutoPropertyGenerator_EmitsAutoPropertyAllPermutationsArityContainer(
        Visibility visibility,
        Accessors accessors,
        ReturnMode returnMode)
    {
        GeneratorDriverRunResult result = GeneratorTestHelper.RunGenerator<AutoPropertyGenerator>(
            $$"""
               using System;
               using Forge.Annotations;
               
               [assembly: AutoPropertyNamingPolicy(NamingPolicy.PascalCase)]

               public partial class Foo<T1, T2> where T1 : class {
                   [AutoProperty(Visibility.{{visibility}}, Accessors.{{accessors}}, ReturnMode.{{returnMode}})]
                   private T1 _value1;
               
                   [AutoProperty(Visibility.{{visibility}}, Accessors.{{accessors}}, ReturnMode.{{returnMode}})]
                   private T2 _value2;
                   
                   [AutoProperty(Visibility.{{visibility}}, Accessors.{{accessors}}, ReturnMode.{{returnMode}})]
                   private Lazy<T1> _lazy;
               }
               """
        );
        
        _ = await Verify(result)
            .HashParameters()
            .UseParameters(visibility, accessors, returnMode);
    }

    [Fact]
    internal async Task AutoPropertyGenerator_EmitsAutoProperty_RespectsContainerTypeStruct() {
        GeneratorDriverRunResult result = GeneratorTestHelper.RunGenerator<AutoPropertyGenerator>(
            $$"""
              using Forge.Annotations;

              [assembly: AutoPropertyNamingPolicy(NamingPolicy.PascalCase)]

              public partial struct Foo {
                  [AutoProperty(Visibility.Public, Accessors.Get)]
                  private int _value;
              }
              """
        );

        _ = await Verify(result)
            .HashParameters();
    }

    [Theory]
    [MemberData(nameof(AllNamingPolicies))]
    internal async Task AutoPropertyGenerator_NamingPolicyRespectsUnderscorePrefix(NamingPolicy namingPolicy) {
        GeneratorDriverRunResult result = GeneratorTestHelper.RunGenerator<AutoPropertyGenerator>(
            $$"""
               using Forge.Annotations;

               [assembly: AutoPropertyNamingPolicy(NamingPolicy.{{namingPolicy}})]

               public partial class Foo {
                   [AutoProperty(Visibility.Public, Accessors.Get)]
                   private int _xmlParser;
               }
               """
        );

        _ = await Verify(result)
            .HashParameters()
            .UseParameters(namingPolicy);
    }
    
    [Theory]
    [MemberData(nameof(AllNamingPolicies))]
    internal async Task AutoPropertyGenerator_NamingPolicyRespectsHungarianPrefix(NamingPolicy namingPolicy) {
        GeneratorDriverRunResult result = GeneratorTestHelper.RunGenerator<AutoPropertyGenerator>(
            $$"""
               using Forge.Annotations;

               [assembly: AutoPropertyNamingPolicy(NamingPolicy.{{namingPolicy}})]

               public partial class Foo {
                   [AutoProperty(Visibility.Public, Accessors.Get)]
                   private int m_xmlParser;
               }
               """
        );

        _ = await Verify(result)
            .HashParameters()
            .UseParameters(namingPolicy);
    }
    
    [Theory]
    [MemberData(nameof(AllNamingPolicies))]
    internal async Task AutoPropertyGenerator_NamingPolicyRespectsNoPrefix(NamingPolicy namingPolicy) {
        GeneratorDriverRunResult result = GeneratorTestHelper.RunGenerator<AutoPropertyGenerator>(
            $$"""
               using Forge.Annotations;

               [assembly: AutoPropertyNamingPolicy(NamingPolicy.{{namingPolicy}})]

               public partial class Foo {
                   [AutoProperty(Visibility.Public, Accessors.Get)]
                   private int xmlParser;
               }
               """
        );

        _ = await Verify(result)
            .HashParameters()
            .UseParameters(namingPolicy);
    }
    
    [Theory]
    [MemberData(nameof(AllNamingPolicies))]
    internal async Task AutoPropertyGenerator_NamingPolicyRespectsAcronym(NamingPolicy namingPolicy) {
        GeneratorDriverRunResult result = GeneratorTestHelper.RunGenerator<AutoPropertyGenerator>(
            $$"""
               using Forge.Annotations;

               [assembly: AutoPropertyNamingPolicy(NamingPolicy.{{namingPolicy}})]

               public partial class Foo {
                   [AutoProperty(Visibility.Public, Accessors.Get)]
                   private int _XMLParser;
               }
               """
        );

        _ = await Verify(result)
            .HashParameters()
            .UseParameters(namingPolicy);
    }
    
    [Theory]
    [MemberData(nameof(AllNamingPolicies))]
    internal async Task AutoPropertyGenerator_NamingPolicyRespectsSingleWord(NamingPolicy namingPolicy) {
        GeneratorDriverRunResult result = GeneratorTestHelper.RunGenerator<AutoPropertyGenerator>(
            $$"""
               using Forge.Annotations;

               [assembly: AutoPropertyNamingPolicy(NamingPolicy.{{namingPolicy}})]

               public partial class Foo {
                   [AutoProperty(Visibility.Public, Accessors.Get)]
                   private int _value;
               }
               """
        );

        _ = await Verify(result)
            .HashParameters()
            .UseParameters(namingPolicy);
    }
    
    [Fact]
    internal async Task AutoPropertyGenerator_NamingPolicyRespectsWhenAttributeAbsent() {
        GeneratorDriverRunResult result = GeneratorTestHelper.RunGenerator<AutoPropertyGenerator>(
            $$"""
               using Forge.Annotations;

               public partial class Foo {
                   [AutoProperty(Visibility.Public, Accessors.Get)]
                   private int _value;
               }
               """
        );

        _ = await Verify(result)
            .HashParameters();
    }
}
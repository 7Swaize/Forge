using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using Forge.Annotations;
using Forge.Generators.Common.Models;
using Forge.Generators.Common.Models.Collections;
using Forge.Generators.Common.Models.Factories;
using Forge.Generators.Features.ZeroCopyParseGenerators.Models;
using Forge.RoslynShared;
using Microsoft.CodeAnalysis;

namespace Forge.Generators.Features.ZeroCopyParseGenerators.Discovery;

internal static class GeneratorTargetParser {
    private static readonly string MagicAttributeName = typeof(MagicAttribute).FullName!;
    private static readonly string BitsAttributeName = typeof(BitsAttribute).FullName!;
    private static readonly string MaxAttributeName = typeof(MaxAttribute).FullName!;
    private static readonly string LengthFromAttributeName = typeof(LengthFromAttribute).FullName!;
    private static readonly string ScaledAttributeName = typeof(ScaledAttribute).FullName!;
    private static readonly string WhenPresentAttributeName = typeof(WhenPresentAttribute).FullName!;
    private static readonly string ExternalContextAttributeName = typeof(ExternalContextAttribute).FullName!;
    private static readonly string PadAttributeName = typeof(PadAttribute).FullName!;
    private static readonly string AlignAttributeName = typeof(AlignAttribute).FullName!;

    private static readonly string FieldOffsetAttributeName = typeof(FieldOffsetAttribute).FullName!;

#if NET7_0_OR_GREATER
    private static readonly string ChecksumName = typeof(ChecksumAttribute<>).FullName!;
    private const string ChecksumSkipLeadingArg = "SkipLeading";
    private const string ChecksumSkipTrailingArg = "SkipTrailing";
#endif
    
    internal static EndianKind GetEndianness(in GeneratorAttributeSyntaxContext ctx) {
        INamedTypeSymbol? targetAttr =
            ctx.SemanticModel.Compilation.GetTypeByMetadataName(typeof(ZeroCopyParserAttribute).FullName!);

        foreach (AttributeData attr in ctx.Attributes) {
            if (SymbolEqualityComparer.Default.Equals(attr.AttributeClass, targetAttr)) {
                return (EndianKind)(int)attr.ConstructorArguments[0].Value!;
            }
        }
        
        return ThrowHelpers.ThrowUnreachable<EndianKind>();
    }

    internal static bool GetDisableReorderOptimizationsFlag(in GeneratorAttributeSyntaxContext ctx) {
        INamedTypeSymbol? targetAttr =
            ctx.SemanticModel.Compilation.GetTypeByMetadataName(typeof(DisableParseOrderOptimizationsAttribute).FullName!);

        foreach (AttributeData attr in ctx.Attributes) {
            if (SymbolEqualityComparer.Default.Equals(attr.AttributeClass, targetAttr)) {
                return true;
            }
        }

        return false;
    }

    internal static bool GetIsBlittable(in GeneratorAttributeSyntaxContext ctx) {
        INamedTypeSymbol? blittableAttr =
            ctx.SemanticModel.Compilation.GetTypeByMetadataName(typeof(BlittableAttribute).FullName!);
        
        foreach (AttributeData attr in ctx.Attributes) {
            if (SymbolEqualityComparer.Default.Equals(attr.AttributeClass, blittableAttr)) {
                return true;
            }
        }

        return ctx.TargetSymbol.As<INamedTypeSymbol>().IsBlittable();
    }

    internal static (LayoutKind Layout, int Pack) GetStructLayoutInformation(in GeneratorAttributeSyntaxContext ctx, bool isBlittable) {
        INamedTypeSymbol? structLayoutAttr =
            ctx.SemanticModel.Compilation.GetTypeByMetadataName(typeof(StructLayoutAttribute).FullName!);
        
        foreach (AttributeData attr in ctx.Attributes) {
            if (SymbolEqualityComparer.Default.Equals(attr.AttributeClass, structLayoutAttr)) {
                return (
                    ExtractNthCompileTimeConstantArg<LayoutKind>(attr, 0),
                    ExtractNthNamedCompileTimeConstantArgOrDefault(attr, nameof(StructLayoutAttribute.Pack), 0)
                );
            }
        }

        return (
            isBlittable ? LayoutKind.Sequential : LayoutKind.Explicit,
            0
        );
    }

    internal static FieldTargetModel ParseField(IFieldSymbol fieldSymbol, TypeReferenceModelFactory typeRefFactory) {
        FieldModel fieldModel = new(fieldSymbol, typeRefFactory);
        ImmutableArray<FieldModifierModel>.Builder modifiers = ImmutableArray.CreateBuilder<FieldModifierModel>();
        
        foreach (AttributeData attr in fieldSymbol.GetAttributes()) {
            if (TryParseModifier(attr, typeRefFactory, out FieldModifierModel parsed)) {
                modifiers.Add(parsed);
            }
        }
        
        if (modifiers.Count == 0) {
            modifiers.Add(FieldModifierModel.None());
        }
        
        return new FieldTargetModel(fieldModel, modifiers.ToImmutable().AsEquatableArray());
    }

    private static bool TryParseModifier(AttributeData attr, TypeReferenceModelFactory typeRefFactory, out FieldModifierModel model) {
        model = default;
        string fullName = attr.AttributeClass!.IsGenericType
            ? attr.AttributeClass!.ConstructUnboundGenericType().GetMetadataStyleFQN(includeGlobal: false)
            : attr.AttributeClass!.GetMetadataStyleFQN(includeGlobal: false);

        if (fullName == MagicAttributeName) {
            model = FieldModifierModel.Magic(ExtractNthCompileTimeConstantArg<ulong>(attr, 0));
        }
        else if (fullName == BitsAttributeName) {
            model = FieldModifierModel.Bits(ExtractNthCompileTimeConstantArg<ulong>(attr, 0));
        }
        else if (fullName == MaxAttributeName) {
            model = FieldModifierModel.Max(ExtractNthCompileTimeConstantArg<ulong>(attr, 0));
        }
        else if (fullName == LengthFromAttributeName) {
            model = FieldModifierModel.LengthFrom(ExtractNthCompileTimeConstantArg<string>(attr, 0));
        }
        else if (fullName == ScaledAttributeName) {
            model = FieldModifierModel.Scaled(ExtractNthTypeArg(attr, 0, typeRefFactory), ExtractNthCompileTimeConstantArg<double>(attr, 1));
        }
        else if (fullName == WhenPresentAttributeName) {
            model = FieldModifierModel.WhenPresent(ExtractNthCompileTimeConstantArg<string>(attr, 0));
        }
        else if (fullName == ExternalContextAttributeName) {
            model = FieldModifierModel.ExternalContext();
        }
        else if (fullName == PadAttributeName) {
            model = FieldModifierModel.Pad(ExtractNthCompileTimeConstantArg<uint>(attr, 0));
        }
        else if (fullName == AlignAttributeName) {
            model = FieldModifierModel.Align(ExtractNthCompileTimeConstantArg<uint>(attr, 0));
        }
        else if (fullName == FieldOffsetAttributeName) {
            model = FieldModifierModel.FieldOffset(ExtractNthCompileTimeConstantArg<int>(attr, 0));
        }
#if NET7_0_OR_GREATER
        else if (fullName == ChecksumName) {
            ITypeReferenceModel checksumCompute = ExtractNthTypeArg(attr, 0, typeRefFactory);
            int skipLeading = ExtractNthNamedCompileTimeConstantArgOrDefault(attr, ChecksumSkipLeadingArg, 0);
            int skipTrailing = ExtractNthNamedCompileTimeConstantArgOrDefault(attr, ChecksumSkipTrailingArg, 0);
            
            model = FieldModifierModel.Checksum(checksumCompute, skipLeading, skipTrailing);
        }
#endif
        else {
            return false;
        }

        return true;
    }

    private static T ExtractNthCompileTimeConstantArg<T>(AttributeData attr, int index) {
        return attr.ConstructorArguments.Length > index
            ? (T)attr.ConstructorArguments[index].Value!
            : ThrowHelpers.ThrowIndexOutOfRangeException<T>();
    }

    private static ITypeReferenceModel ExtractNthTypeArg(AttributeData attr, int index, TypeReferenceModelFactory typeRefFactory) {
        return attr.ConstructorArguments.Length > index
            ? typeRefFactory.CreateOrGetTypeReferenceModel((ITypeSymbol)attr.ConstructorArguments[index].Value!)
            : ThrowHelpers.ThrowIndexOutOfRangeException<ITypeReferenceModel>();
    }
    
    private static T ExtractNthNamedCompileTimeConstantArgOrDefault<T>(AttributeData attr, string name, T defaultValue) {
        foreach (KeyValuePair<string, TypedConstant> named in attr.NamedArguments) {
            if (named.Key == name && named.Value.Value is T value) {
                return value;
            }
        }
        
        return defaultValue;
    }
}
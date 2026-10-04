using System;
using Forge.Generators.Common.Models;
using Forge.RoslynShared;

namespace Forge.Generators.Features.ZeroCopyParseGenerators.Models;

internal readonly record struct FieldTargetModel {
    internal FieldModel FieldModel { get; init; }
    internal FieldModifierKind Kind { get; init; }
    
    internal ITypeReferenceModel TypeArg { get; init; }
    internal string StringArg { get; init; }
    internal ulong ULongArg { get; init; }
    internal double DoubleArg { get; init; }
    internal int IntArg1 { get; init; }
    internal int IntArg2 { get; init; }
    
    internal static FieldTargetModel None() => 
        new() { Kind = FieldModifierKind.None };
    
    internal static FieldTargetModel Magic(ulong shouldEqual) =>
        new() { Kind = FieldModifierKind.Magic, ULongArg = shouldEqual };
 
    internal static FieldTargetModel Bits(ulong bits) =>
        new() { Kind = FieldModifierKind.Bits, ULongArg = bits };
 
    internal static FieldTargetModel Max(ulong max) =>
        new() { Kind = FieldModifierKind.Max, ULongArg = max };
 
    internal static FieldTargetModel LengthFrom(string fieldName) =>
        new() { Kind = FieldModifierKind.LengthFrom, StringArg = fieldName };
 
    internal static FieldTargetModel Scaled(ITypeReferenceModel wireType, double factor) =>
        new() { Kind = FieldModifierKind.Scaled, TypeArg = wireType, DoubleArg = factor };
 
    internal static FieldTargetModel WhenPresent(string fieldName) =>
        new() { Kind = FieldModifierKind.WhenPresent, StringArg = fieldName };
 
    internal static FieldTargetModel ExternalContext() =>
        new() { Kind = FieldModifierKind.ExternalContext };
 
    internal static FieldTargetModel Pad(uint bytes) =>
        new() { Kind = FieldModifierKind.Pad, ULongArg = bytes };
 
    internal static FieldTargetModel Align(uint boundary) =>
        new() { Kind = FieldModifierKind.Align, ULongArg = boundary };
 
    internal static FieldTargetModel Checksum(ITypeReferenceModel algorithm, int skipLeading = 0, int skipTrailing = 0) =>
        new() { Kind = FieldModifierKind.Checksum, TypeArg = algorithm, IntArg1 = skipLeading, IntArg2 = skipTrailing };
 
    internal TResult Accept<TVisitor, TResult>(ref TVisitor visitor) where TVisitor : struct, IFieldModifierVisitor<TResult> {
        return Kind switch {
            FieldModifierKind.None => visitor.VisitNone(in this),
            FieldModifierKind.Magic => visitor.VisitMagic(in this),
            FieldModifierKind.Bits => visitor.VisitBits(in this),
            FieldModifierKind.Max => visitor.VisitMax(in this),
            FieldModifierKind.LengthFrom => visitor.VisitLengthFrom(in this),
            FieldModifierKind.Scaled => visitor.VisitScaled(in this),
            FieldModifierKind.WhenPresent => visitor.VisitWhenPresent(in this),
            FieldModifierKind.ExternalContext => visitor.VisitExternalContext(in this),
            FieldModifierKind.Pad => visitor.VisitPad(in this),
            FieldModifierKind.Align => visitor.VisitAlign(in this),
            FieldModifierKind.Checksum => visitor.VisitChecksum(in this),
            _ => ThrowHelpers.ThrowUnhandledBranch<TResult>(Kind)
        };
    }
}

internal interface IFieldModifierVisitor<out TResult> {
    TResult VisitNone(in FieldTargetModel model);
    TResult VisitMagic(in FieldTargetModel model);
    TResult VisitBits(in FieldTargetModel model);
    TResult VisitMax(in FieldTargetModel model);
    TResult VisitLengthFrom(in FieldTargetModel model);
    TResult VisitScaled(in FieldTargetModel model);
    TResult VisitWhenPresent(in FieldTargetModel model);
    TResult VisitExternalContext(in FieldTargetModel model);
    TResult VisitPad(in FieldTargetModel model);
    TResult VisitAlign(in FieldTargetModel model);
    TResult VisitChecksum(in FieldTargetModel model);
}


[Flags]
internal enum FieldModifierKind : uint {
    None = 0,
 
    Magic = 1u << 0,
    Max = 1u << 1,
    Checksum = 1u << 2,
 
    Bits = 1u << 3,
    Pad = 1u << 4,
    Align = 1u << 5,
 
    LengthFrom = 1u << 6,
    Scaled = 1u << 7,
 
    WhenPresent = 1u << 8,
    ExternalContext = 1u << 9,
}
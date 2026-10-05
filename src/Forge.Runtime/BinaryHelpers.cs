using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Forge.Runtime;

public static class BinaryHelpers {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref byte Ref(ReadOnlySpan<byte> s) => ref Unsafe.AsRef(in MemoryMarshal.GetReference(s));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short I16BE(ref byte r, int o) =>
        BitConverter.IsLittleEndian
            ? BinaryPrimitives.ReverseEndianness(Unsafe.ReadUnaligned<short>(ref Unsafe.Add(ref r, o)))
            : Unsafe.ReadUnaligned<short>(ref Unsafe.Add(ref r, o));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort U16BE(ref byte r, int o) =>
        BitConverter.IsLittleEndian
            ? BinaryPrimitives.ReverseEndianness(Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref r, o)))
            : Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref r, o));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int I32BE(ref byte r, int o) =>
        BitConverter.IsLittleEndian
            ? BinaryPrimitives.ReverseEndianness(Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref r, o)))
            : Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref r, o));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint U32BE(ref byte r, int o) =>
        BitConverter.IsLittleEndian
            ? BinaryPrimitives.ReverseEndianness(Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref r, o)))
            : Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref r, o));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long I64BE(ref byte r, int o) =>
        BitConverter.IsLittleEndian
            ? BinaryPrimitives.ReverseEndianness(Unsafe.ReadUnaligned<long>(ref Unsafe.Add(ref r, o)))
            : Unsafe.ReadUnaligned<long>(ref Unsafe.Add(ref r, o));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong U64BE(ref byte r, int o) =>
        BitConverter.IsLittleEndian
            ? BinaryPrimitives.ReverseEndianness(Unsafe.ReadUnaligned<ulong>(ref Unsafe.Add(ref r, o)))
            : Unsafe.ReadUnaligned<ulong>(ref Unsafe.Add(ref r, o));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float F32BE(ref byte r, int o) {
        int v = I32BE(ref r, o);
        return Unsafe.As<int, float>(ref v);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double F64BE(ref byte r, int o) =>
        BitConverter.Int64BitsToDouble(I64BE(ref r, o));
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short I16LE(ref byte r, int o) =>
        BitConverter.IsLittleEndian
            ? Unsafe.ReadUnaligned<short>(ref Unsafe.Add(ref r, o))
            : BinaryPrimitives.ReverseEndianness(Unsafe.ReadUnaligned<short>(ref Unsafe.Add(ref r, o)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort U16LE(ref byte r, int o) =>
        BitConverter.IsLittleEndian
            ? Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref r, o))
            : BinaryPrimitives.ReverseEndianness(Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref r, o)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int I32LE(ref byte r, int o) =>
        BitConverter.IsLittleEndian
            ? Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref r, o))
            : BinaryPrimitives.ReverseEndianness(Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref r, o)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint U32LE(ref byte r, int o) =>
        BitConverter.IsLittleEndian
            ? Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref r, o))
            : BinaryPrimitives.ReverseEndianness(Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref r, o)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long I64LE(ref byte r, int o) =>
        BitConverter.IsLittleEndian
            ? Unsafe.ReadUnaligned<long>(ref Unsafe.Add(ref r, o))
            : BinaryPrimitives.ReverseEndianness(Unsafe.ReadUnaligned<long>(ref Unsafe.Add(ref r, o)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong U64LE(ref byte r, int o) =>
        BitConverter.IsLittleEndian
            ? Unsafe.ReadUnaligned<ulong>(ref Unsafe.Add(ref r, o))
            : BinaryPrimitives.ReverseEndianness(Unsafe.ReadUnaligned<ulong>(ref Unsafe.Add(ref r, o)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float F32LE(ref byte r, int o) {
        if (BitConverter.IsLittleEndian) {
            return Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref r, o));
        }
        
        int v = I32LE(ref r, o);
        return Unsafe.As<int, float>(ref v);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double F64LE(ref byte r, int o) =>
        BitConverter.Int64BitsToDouble(I64LE(ref r, o));
}
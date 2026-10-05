using System;

namespace Forge.Annotations;

/// <summary>
/// Marks a <see langword="struct"/> as a wire-format description. A source generator emits a
/// zero-allocation <c>TryParse</c> method for it that reads directly from a
/// <see cref="ReadOnlySpan{T}"/> of bytes.
/// </summary>
/// <remarks>
/// <para>
/// Fields are parsed in declaration order, and the generator tracks a running offset, so field
/// order in the source must match field order on the wire.
/// </para>
/// <para>
/// The struct should be declared <see langword="partial"/> (and usually <see langword="readonly"/> and <see langword="ref"/>).
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Struct)]
public sealed class ZeroCopyParserAttribute(Endian endian) : Attribute {
    /// <summary>
    /// The byte order applied to every multi-byte field in the struct.
    /// </summary>
    public Endian Endian { get; private set; } = endian;
}


/// <summary>
/// Disables front-loading of initial checks to early return. Reordering avoids initial, unnecessary parsing in the event
/// that an asserted condition fails. Reordering has no effect on the structure's internal representation.
/// Reordering is enabled by default.
/// </summary>
[AttributeUsage(AttributeTargets.Struct)]
public sealed class DisableParseOrderOptimizationsAttribute : Attribute { }


/// <summary>
/// Requires the annotated field to equal a fixed value on the wire. If the value read does not
/// match, the generated <c>TryParse</c> fails instead of producing a result.
/// </summary>
/// <remarks>
/// The check runs as soon as the field is read. If parse order is allowed to be optimized (default),
/// malformed input is rejected before later fields (or additional data) are needed.
/// </remarks>
[AttributeUsage(AttributeTargets.Field)]
public sealed class MagicAttribute(ulong shouldEqual) : Attribute {
    /// <summary>
    /// The exact value the field must have. It is compared after endianness conversion, so it
    /// should be written as the logical value (e.g. <c>0xA55A</c>), not as raw bytes.
    /// </summary>
    public ulong ShouldEqual { get; private set; } = shouldEqual;
}


/// <summary>
/// Declares the annotated field as a bit-packed value that occupies a given number of bits
/// rather than a whole number of bytes.
/// </summary>
/// <remarks>
/// Consecutive <see cref="BitsAttribute"/> fields are packed into shared bytes, starting from the
/// least significant bit. Their widths must add up to a multiple of 8.
/// </remarks>
[AttributeUsage(AttributeTargets.Field)]
public sealed class BitsAttribute(int bits) : Attribute {
    /// <summary>
    /// The width of the field, in bits.
    /// </summary>
    public int Bits { get; private set; } = bits;
}

/// <summary>
/// Sets an inclusive upper bound for the annotated field's value. Values above the bound cause the
/// generated <c>TryParse</c> to fail.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public sealed class MaxAttribute(ulong max) : Attribute {
    /// <summary>
    /// The largest value the field may have (inclusive).
    /// </summary>
    public ulong Max { get; private set; } = max;
}


/// <summary>
/// Marks a <see cref="ReadOnlySpan{T}"/> field whose length in bytes is given by an earlier
/// integer field. The span is a slice of the input buffer, not a copy.
/// </summary>
/// <remarks>
/// The referenced field must be declared before the annotated field and must be an integer type.
/// The value is only valid for as long as the buffer that was parsed.
/// </remarks>
[AttributeUsage(AttributeTargets.Field)]
public sealed class LengthFromAttribute(string fieldName) : Attribute {
    /// <summary>
    /// The name of the earlier field that holds the length. Use <see langword="nameof"/>.
    /// </summary>
    public string FieldName { get; private set; } = fieldName;
}


/// <summary>
/// Reads an integer from the wire and multiplies it by a factor, storing the result in the
/// annotated double or float field. Used for fixed-point encodings such as "volts in hundredths".
/// </summary>
/// <remarks>
/// The wire size of the field is determined by <see cref="WireType"/>, not by the declared
/// field type.
/// </remarks>
[AttributeUsage(AttributeTargets.Field)]
public sealed class ScaledAttribute(Type wireType, double factor) : Attribute {
    /// <summary>
    /// The type stored on the wire (e.g. <c>typeof(short)</c>).
    /// </summary>
    public Type WireType { get; private set; } = wireType;
    
    /// <summary>
    /// The value the wire type is multiplied by to produce the logical value.
    /// </summary>
    public double Factor { get; private set; } = factor;
}


/// <summary>
/// Makes the annotated field optional: it is read only when an earlier <see cref="bool"/> field
/// is <see langword="true"/>. When absent, it is skipped without consuming any bytes and holds
/// its default value.
/// </summary>
/// <remarks>
/// The referenced field must be declared before the annotated field. It may be a parsed field
/// (such as a flag bit) or an <see cref="ExternalContextAttribute"/> field supplied by the caller.
/// </remarks>
[AttributeUsage(AttributeTargets.Field)]
public sealed class WhenPresentAttribute(string fieldName) : Attribute {
    /// <summary>
    /// The name of the earlier <see cref="bool"/> field that controls whether this field is present.
    /// </summary>
    public string FieldName { get; private set; } = fieldName;
}


/// <summary>
/// Marks a field that is not read from the wire. Instead, the caller supplies its value as a
/// parameter of the generated <c>TryParse</c> method.
/// </summary>
/// <remarks>
/// Used to pass information parsed elsewhere, such as flags from an outer frame header
/// that control which optional fields appear in an inner body.
/// </remarks>
[AttributeUsage(AttributeTargets.Field)]
public sealed class ExternalContextAttribute : Attribute { }


/// <summary>
/// Specifies the number of padding bytes that exist before the field. These bytes are skipped during parsing.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public sealed class PadAttribute(int bytes) : Attribute {
    /// <summary>
    /// Gets the number of padding bytes that exist before the field.
    /// </summary>
    public int Bytes { get; private set; } = bytes;
}


/// <summary>
/// Specifies the byte boundary to which the annotated field must be aligned to.
/// During parsing this skips to the next multiple of <c>boundary</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public sealed class AlignAttribute(int boundary) : Attribute {
    /// <summary>
    /// Gets the alignment boundary, in bytes, required for the field's offset.
    /// </summary>
    public int Boundary { get; private set; } = boundary;
}


#if NET7_0_OR_GREATER
/// <summary>
/// Marks a field as a checksum over the preceding bytes. The generated <c>TryParse</c> computes
/// the checksum with <typeparamref name="T"/> and fails if it does not match the value on the wire.
/// </summary>
/// <typeparam name="T">The algorithm used to compute the checksum.</typeparam>
/// <remarks>
/// The checksummed range runs from the start of the input (after <see cref="SkipLeading"/> bytes)
/// up to the checksum field itself.
/// </remarks>
[AttributeUsage(AttributeTargets.Field)]
public sealed class ChecksumAttribute<T> : Attribute where T : IChecksumCompute {
    /// <summary>
    /// The number of bytes at the start of the input to exclude from the checksum.
    /// </summary>
    public int SkipLeading { get; set; }
    
    /// <summary>
    /// The number of bytes immediately before the checksum field to exclude from the checksum.
    /// </summary>
    public int SkipTrailing { get; set; }
}

/// <summary>
/// Defines a checksum algorithm for use with <see cref="ChecksumAttribute{T}"/>.
/// </summary>
public interface IChecksumCompute {
    /// <summary>
    /// Computes the checksum of <paramref name="data"/>.
    /// </summary>
    /// <param name="data">The bytes to checksum.</param>
    /// <returns>
    /// The checksum, widened to <see cref="ulong"/>. Narrower checksums (such as CRC-16) occupy
    /// the low bits.
    /// </returns>
    static abstract ulong Compute(ReadOnlySpan<byte> data);
}
#endif


/// <summary>
/// Byte order of multi-byte values on the wire.
/// </summary>
public enum Endian {
    /// <summary>Most significant byte first (network byte order).</summary>
    Big = 0,
 
    /// <summary>Least significant byte first.</summary>
    Little = 1
}


/// <summary>
/// The outcome of a generated <c>TryParse</c> call.
/// </summary>
public enum ParseStatus {
    /// <summary>The input was parsed successfully.</summary>
    Ok = 0,
 
    /// <summary>The input is a valid prefix but too short. Append more bytes and call again.</summary>
    NeedMoreData = 1,
 
    /// <summary>A <see cref="MagicAttribute"/> field did not match.</summary>
    BadMagic = 2,
 
    /// <summary>A <see cref="MaxAttribute"/> bound was exceeded.</summary>
    TooLarge = 3,
 
    /// <summary>A checksum field did not match the computed checksum.</summary>
    BadChecksum = 4,
 
    /// <summary>
    /// The input is structurally invalid and more bytes will not fix it (for example a body shorter than its own
    /// length fields claim, or a negative length).
    /// </summary>
    Malformed = 5,
}
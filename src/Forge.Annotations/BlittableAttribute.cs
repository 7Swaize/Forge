using System;

namespace Forge.Annotations;

/// <summary>
/// Declares that the annotated struct must be blittable.
/// </summary>
/// <remarks>
/// <para>
/// Every instance field of the struct must itself be blittable, otherwise error <c>FRG0600</c> is reported on the
/// offending field. The following types are blittable:
/// </para>
/// <list type="bullet">
/// <item><description>
/// <see cref="byte"/>, <see cref="sbyte"/>, <see cref="short"/>, <see cref="ushort"/>, <see cref="int"/>,
/// <see cref="uint"/>, <see cref="long"/>, <see cref="ulong"/>, <see cref="float"/> and <see cref="double"/>.
/// </description></item>
/// <item><description>
/// <see cref="IntPtr"/> and <see cref="UIntPtr"/>, including their <see langword="nint"/> and
/// <see langword="nuint"/> aliases.
/// </description></item>
/// <item><description>Enum types.</description></item>
/// <item><description>Pointers, including function pointers.</description></item>
/// <item><description>Fixed-size buffers of blittable element types.</description></item>
/// <item><description>Structs whose instance fields are all blittable, to any depth of nesting.</description></item>
/// </list>
/// <para>
/// Static and constant fields are not part of an instance's memory and are ignored.
/// </para>
/// <para>
/// A struct that declares <see cref="System.Runtime.InteropServices.StructLayoutAttribute"/> with
/// <see cref="System.Runtime.InteropServices.LayoutKind.Auto"/> reports error <c>FRG0601</c>, because the runtime
/// is then free to reorder the fields and the layout cannot be relied on. A struct with no
/// <see cref="System.Runtime.InteropServices.StructLayoutAttribute"/> uses sequential layout, which is accepted, as is
/// <see cref="System.Runtime.InteropServices.LayoutKind.Explicit"/>.
/// </para>
/// <para>
/// The attribute has no effect at run time.
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Struct)]
public sealed class BlittableAttribute : Attribute { }
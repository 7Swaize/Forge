using System;

namespace Forge.Annotations;

/// <summary>
/// Requires the argument passed to the annotated parameter to be a compile-time constant at every call site.
/// </summary>
/// <remarks>
/// <para>
/// Passing a value that is not a constant, such as a variable or a method result, reports error <c>FRG0300</c>.
/// Literals, <see langword="const"/> members and constant expressions are accepted. The check applies to method
/// invocations.
/// </para>
/// <para>
/// Only <see cref="bool"/>, <see cref="char"/>, the integral and floating-point numeric types,
/// <see cref="decimal"/>, <see cref="string"/> and enum types can be compile-time constants. Annotating a
/// parameter of any other type reports warning <c>FRG0301</c>, because no argument could ever satisfy it.
/// </para>
/// <para>
/// The attribute has no effect at run time.
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class CompileTimeConstantAttribute : Attribute { }
using System;

namespace Forge.Annotations;

/// <summary>
/// Disallows local variable declarations in the annotated method.
/// </summary>
/// <remarks>
/// <para>
/// Each local variable declaration statement in the method body, such as <c>int x = 0;</c> or
/// <c>var y = Compute();</c>, reports error <c>FRG0200</c>.
/// </para>
/// <para>
/// Lambdas and local functions declared inside the method are not inspected.
/// </para>
/// <para>
/// The attribute has no effect at run time.
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Method)]
public sealed class NoLocalVariablesAttribute : Attribute { }
using System;

namespace Forge.Annotations;

/// <summary>
/// Sets the naming policy used to derive property names for every <see cref="AutoPropertyAttribute"/> in the assembly.
/// </summary>
/// <remarks>
/// <para>
/// Apply it once at assembly level. When the attribute is absent, <see cref="NamingPolicy.PascalCase"/> is used.
/// If it is applied more than once, only one of the declarations takes effect.
/// </para>
/// <para>
/// To build the property name, leading underscores and the prefixes <c>m_</c>, <c>f_</c>, <c>s_</c> and <c>t_</c>
/// are removed from the field name. The rest is split into words at underscores, hyphens, lower-to-upper case
/// changes, acronym boundaries and letter/digit boundaries, then reassembled according to the policy.
/// </para>
/// <para>
/// If the resulting name would be identical to the field name, <c>Value</c> is appended so the property does not
/// collide with the field (for example, a field named <c>health</c> becomes <c>healthValue</c> under
/// <see cref="NamingPolicy.CamelCase"/>).
/// </para>
/// </remarks>
/// <param name="namingPolicy">The naming policy to apply.</param>
[AttributeUsage(AttributeTargets.Assembly)]
public sealed class AutoPropertyNamingPolicyAttribute(NamingPolicy namingPolicy) : Attribute {
    /// <summary>
    /// Gets the naming policy to apply.
    /// </summary>
    public NamingPolicy NamingPolicy { get; private set; } = namingPolicy;
}

/// <summary>
/// Specifies how generated property names are formatted.
/// </summary>
public enum NamingPolicy : byte {
    /// <summary>
    /// Each word starts with an uppercase letter. This is the default.
    /// </summary>
    PascalCase = 0,

    /// <summary>
    /// The first word is lowercase and each following word starts with an uppercase letter.
    /// </summary>
    CamelCase = 1,

    /// <summary>
    /// All words are lowercase and separated by underscores.
    /// </summary>
    SnakeCase = 2,
}



using System;

namespace Forge.Annotations;

/// <summary>
/// Generates a property that exposes the annotated field.
/// </summary>
/// <remarks>
/// <para>
/// The containing type must be declared <see langword="partial"/> (<c>FRG0001</c>). The property is
/// emitted into a generated partial declaration of that type.
/// </para>
/// <para>
/// The property name is derived from the field name using the naming policy set by
/// <see cref="AutoPropertyNamingPolicyAttribute"/>, which is PascalCase when the attribute is not present.
/// </para>
/// <para>
/// <see cref="Forge.Annotations.ReturnMode.RefStruct"/> and <see cref="Forge.Annotations.ReturnMode.RefReadonlyStruct"/>
/// can only be combined with <see cref="Forge.Annotations.Accessors.Get"/> (<c>FRG0100</c>).
/// </para>
/// </remarks>
/// <param name="visibility">The accessibility of the generated property.</param>
/// <param name="accessors">Which accessors the generated property declares.</param>
/// <param name="returnMode">How the getter returns the field. Defaults to <see cref="Forge.Annotations.ReturnMode.Default"/>.</param>
[AttributeUsage(AttributeTargets.Field)]
public sealed class AutoPropertyAttribute(Visibility visibility, Accessors accessors, ReturnMode returnMode = ReturnMode.Default) : Attribute {
    /// <summary>
    /// Gets the accessibility of the generated property.
    /// </summary>
    public Visibility Visibility { get; private set; } = visibility;
    
    /// <summary>
    /// Gets which accessors the generated property declares.
    /// </summary>
    public Accessors Accessors { get; private set; } = accessors;
    
    /// <summary>
    /// Gets how the generated accessor returns the backing field (i.e. byref or not)
    /// </summary>
    public ReturnMode ReturnMode { get; private set; } = returnMode;
}


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


/// <summary>
/// Specifies which accessors an <see cref="AutoPropertyAttribute"/> property declares.
/// </summary>
public enum Accessors : byte {
    /// <summary>
    /// A read-only property with an expression-bodied getter.
    /// </summary>
    Get = 0,

    /// <summary>
    /// A write-only property with a setter.
    /// </summary>
    Set = 1,

    /// <summary>
    /// A property with a getter and a setter.
    /// </summary>
    GetAndSet = 2,

    /// <summary>
    /// A property with a getter and an init accessor.
    /// </summary>
    GetAndInit = 3
}


/// <summary>
/// Specifies how the getter of an <see cref="AutoPropertyAttribute"/> property returns the field.
/// </summary>
/// <remarks>
/// The byref modes are intended for struct-typed fields, where they avoid copying the value.
/// They are only valid together with <see cref="Accessors.Get"/>.
/// </remarks>
public enum ReturnMode : byte {
    /// <summary>
    /// Returns the field by value.
    /// </summary>
    Default = 0,

    /// <summary>
    /// Returns the field via the `ref` keyword.
    /// </summary>
    RefStruct = 1,

    /// <summary>
    /// Returns the field as a readonly byref.
    /// </summary>
    RefReadonlyStruct = 2
}


// This preserves the mapping between Microsoft.CodeAnalysis.Accessibility
/// <summary>
/// Specifies the accessibility of a generated member.
/// </summary>
public enum Visibility : byte {
    /// <summary>
    /// Accessible only within the containing type (<see langword="private"/>).
    /// </summary>
    Private = 1,

    /// <summary>
    /// Accessible within the containing type and derived types in the same assembly (<see langword="private protected"/>).
    /// </summary>
    PrivateProtected = 2,

    /// <summary>
    /// Accessible within the containing type and derived types (<see langword="protected"/>).
    /// </summary>
    Protected = 3,

    /// <summary>
    /// Accessible within the same assembly (<see langword="internal"/>).
    /// </summary>
    Internal = 4,

    /// <summary>
    /// Accessible within the same assembly, and from derived types in any assembly (<see langword="protected internal"/>).
    /// </summary>
    ProtectedInternal = 5,

    /// <summary>
    /// Accessible from anywhere (<see langword="public"/>).
    /// </summary>
    Public = 6
}

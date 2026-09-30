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
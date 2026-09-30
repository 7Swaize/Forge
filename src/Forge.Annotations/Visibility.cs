namespace Forge.Annotations;

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

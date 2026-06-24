namespace Forge.Annotations;

// This preserves the mapping between Microsoft.CodeAnalysis.Accessibility
public enum Visibility : byte {
    Private = 1,
    PrivateProtected = 2,
    Protected = 3,
    Internal = 4,
    ProtectedInternal = 5,
    Public = 6
}
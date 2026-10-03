using System;

namespace Forge.Annotations;

[AttributeUsage(
    AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Property |
    AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface | AttributeTargets.Delegate
)]
public sealed class NoAllocAttribute(AllocKinds banned = AllocKinds.Heap) : Attribute {
    public AllocKinds Banned { get; private set; } = banned;
}


[AttributeUsage(
    AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Property |
    AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface | AttributeTargets.Delegate
)]
public sealed class AllocAllowedAttribute(AllocKinds allowed = AllocKinds.All) : Attribute {
    public AllocKinds Allowed { get; private set; } = allowed;    
}


[Flags]
public enum AllocKinds {
    None = 0,
    Heap = 1 << 0,
    Stack = 1 << 1,
    Native = 1 << 2,
    All = Heap | Stack | Native
}
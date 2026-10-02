using System;

namespace Forge.Annotations;

public class NoAllocAttribute(AllocKinds banned = AllocKinds.Heap) : Attribute {
    public AllocKinds Banned { get; private set; } = banned;
}


[Flags]
public enum AllocKinds {
    None = 0,
    Heap = 1 << 0,
    Stack = 1 << 1,
    Native = 1 << 2,
    All = Heap | Stack | Native
}
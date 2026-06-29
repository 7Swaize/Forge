using System;

namespace Forge.Annotations;

[AttributeUsage(AttributeTargets.Assembly)]
public sealed class AutoPropertyNamingPolicyAttribute(NamingPolicy namingPolicy) : Attribute {
    public NamingPolicy NamingPolicy { get; private set; } = namingPolicy;
}

public enum NamingPolicy : byte {
    PascalCase = 0,
    CamelCase = 1,
    SnakeCase = 2,
}


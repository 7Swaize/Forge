using System;

namespace Forge.Annotations;

[AttributeUsage(
    AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Property | AttributeTargets.Field |
    AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface
)]
public sealed class RemoveByAttribute(string date, string? reason = null) : Attribute {
    public string Date { get; private set; } = date;
    public string? Reason { get; private set; } = reason;
}
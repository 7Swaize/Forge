using System;

namespace Forge.Annotations;

/// <summary>
/// Marks code as temporary and schedules it for removal on a specific date.
/// </summary>
/// <remarks>
/// <para>
/// The Forge analyzer evaluates the date at compile time:
/// </para>
/// <list type="bullet">
///   <item>
///     <description>
///       Before the date, a warning (<c>FRG0500</c>) reminds you to plan the cleanup.
///     </description>
///   </item>
///   <item>
///     <description>
///       On or after the date, an error (<c>FRG0501</c>) fails the build until the code is removed or refactored.
///     </description>
///   </item>
///   <item>
///     <description>
///       A date that is not in <c>yyyy-MM-dd</c> format produces an error (<c>FRG0502</c>) on the attribute itself.
///     </description>
///   </item>
/// </list>
/// </remarks>
[AttributeUsage(
    AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Property | AttributeTargets.Field |
    AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface
)]
public sealed class RemoveByAttribute(string date, string? reason = null) : Attribute {
    /// <summary>
    /// The date by which the annotated code must be removed, in <c>yyyy-MM-dd</c> format
    /// (for example, <c>2026-12-31</c>).
    /// </summary>
    public string Date { get; private set; } = date;

    /// <summary>
    /// An optional explanation of why the code is temporary and what needs to happen before it can be removed.
    /// </summary>
    public string? Reason { get; private set; } = reason;
}
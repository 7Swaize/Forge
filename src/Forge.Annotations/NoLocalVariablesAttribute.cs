using System;

namespace Forge.Annotations;

[AttributeUsage(AttributeTargets.Method)]
public sealed class NoLocalVariablesAttribute : Attribute { }
using System;

namespace Forge.Annotations;

[AttributeUsage(AttributeTargets.Parameter)]
public sealed class CompileTimeConstantAttribute : Attribute { }
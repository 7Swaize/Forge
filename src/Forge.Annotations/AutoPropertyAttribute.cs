using System;

namespace Forge.Annotations;

[AttributeUsage(AttributeTargets.Field)]
public sealed class AutoPropertyAttribute(Visibility visibility, Accessors accessors, ReturnMode returnMode = ReturnMode.Default) : Attribute {
    public Visibility Visibility { get; private set; } = visibility;
    public Accessors Accessors { get; private set; } = accessors;
    public ReturnMode ReturnMode { get; private set; } = returnMode;
}


public enum Accessors : byte {
    Get = 0,
    Set = 1,
    GetAndSet = 2,
    GetAndInit = 3
}


public enum ReturnMode : byte {
    Default = 0,
    RefStruct = 1,
    RefReadonlyStruct = 2
}
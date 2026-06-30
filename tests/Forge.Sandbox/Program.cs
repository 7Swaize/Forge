using System;
using Forge.Annotations;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor/
#pragma warning disable CS0169 // Field is never used
#pragma warning disable CS0219 // Variable is assigned but its value is never used


[assembly: AutoPropertyNamingPolicy(NamingPolicy.PascalCase)]


public static class Program {
    public static void Main(string[] args) { }
}


public partial class Test {
    [AutoProperty(Visibility.Internal, Accessors.Get, ReturnMode.Default)]
    private Guid _guid;
}


public partial class Foo<T1, T2> where T1 : class {
    [AutoProperty(Visibility.Internal, Accessors.Get, ReturnMode.RefReadonlyStruct)]
    private T1 _value1;

    [AutoProperty(Visibility.Internal, Accessors.Get, ReturnMode.RefReadonlyStruct)]
    private T2 _value2;
    
    [AutoProperty(Visibility.Public, Accessors.Get)]
    private int _xmlParser;
}



public partial struct Foo {
    [AutoProperty(Visibility.Public, Accessors.Get)]
    private int _value;
}
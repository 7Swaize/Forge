using System;
using System.Collections.Generic;
using Forge.Annotations;

[assembly: AutoPropertyNamingPolicy(NamingPolicy.PascalCase)]

public static class Program {
    public static void Main(string[] args) { }
}


public partial class Test {
    [AutoProperty(Visibility.Internal, Accessors.Get, ReturnMode.RefReadonlyStruct)]
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
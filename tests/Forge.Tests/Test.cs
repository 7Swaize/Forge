using System;
using Forge.Annotations;
using Xunit;

[assembly: AutoPropertyNamingPolicy(NamingPolicy.PascalCase)]

namespace Forge.Tests;

public partial class Test {
    [AutoProperty(Visibility.Internal, Accessors.Get, ReturnMode.RefReadonlyStruct)]
    private Guid _guid;
}

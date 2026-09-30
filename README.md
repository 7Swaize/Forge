# Forge

A collection of Roslyn generators and analyzers. These are triggered via attributes.

If you would like to propose an attribute, please open an issue. I will review the proposal and (probably) implement it.
## Installing Forge

```bash
dotnet add package ForgeGen
```

## Attributes

All attributes live in the `Forge.Annotations` namespace.

### `[AutoProperty]`

Generates a property for a field. The containing type must be `partial`.

```csharp
using System.Numerics;
using Forge.Annotations;

public partial class Player {
    [AutoProperty(Visibility.Public, Accessors.GetAndSet)]
    private int _health;

    [AutoProperty(Visibility.Public, Accessors.Get, ReturnMode.RefReadonlyStruct)]
    private Vector3 _position;
}

// Generated:
// public int Health { get => _health; set => _health = value; }
// public ref readonly Vector3 Position => ref _position;
```

- `Accessors`: `Get`, `Set`, `GetAndSet` or `GetAndInit`.
- `ReturnMode` (optional): `RefStruct` or `RefReadonlyStruct` return the field by reference to avoid copying structs. Only valid with `Accessors.Get`.

### `[assembly: AutoPropertyNamingPolicy]`

Sets how `[AutoProperty]` names its properties. Defaults to `PascalCase`.

```csharp
[assembly: AutoPropertyNamingPolicy(NamingPolicy.SnakeCase)]

// A field named _maxHealth now generates a property named max_health.
```

Options are `PascalCase`, `CamelCase` and `SnakeCase`. Leading underscores and the prefixes `m_`, `f_`, `s_` and `t_` are stripped from the field name first.

### `[CompileTimeConstant]`

Requires the argument passed to a parameter to be a compile-time constant.

```csharp
static void Log([CompileTimeConstant] string message) { }

Log("started");   // OK
Log(userInput);   // error FRG0300
```

### `[NoLocalVariables]`

Disallows local variable declarations inside a method.

```csharp
[NoLocalVariables]
int Add(int a, int b) {
    int sum = a + b;   // error FRG0200
    return sum;
}
```
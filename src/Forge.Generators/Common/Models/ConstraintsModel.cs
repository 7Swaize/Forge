using Forge.Generators.Common.Models.Collections;

namespace Forge.Generators.Common.Models;

internal sealed record ConstraintsModel {
    internal ConstraintsModel(string typeName, EquatableArray<string> values) {
        TypeName = typeName;
        Values = values;
    }

    public override string ToString() {
        return $"where {TypeName} : {string.Join(", ", Values)}";
    }

    internal string TypeName { get; }
    internal EquatableArray<string> Values { get; }
}
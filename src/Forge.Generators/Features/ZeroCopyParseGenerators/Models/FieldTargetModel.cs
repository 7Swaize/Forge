using Forge.Generators.Common.Models;
using Forge.Generators.Common.Models.Collections;

namespace Forge.Generators.Features.ZeroCopyParseGenerators.Models;

internal readonly record struct FieldTargetModel {
    internal FieldTargetModel(FieldModel model, EquatableArray<FieldModifierModel> modifiers) {
        FieldModel = model;
        Modifiers = modifiers;
    }
    
    internal FieldModel FieldModel { get; init; }
    internal EquatableArray<FieldModifierModel> Modifiers { get; init; }
}
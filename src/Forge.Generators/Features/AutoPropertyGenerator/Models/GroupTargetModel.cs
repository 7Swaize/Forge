using System.Collections.Immutable;
using Forge.Generators.Common.Models;
using Forge.Generators.Common.Models.Collections;

namespace Forge.Generators.Features.AutoPropertyGenerator.Models;

internal sealed record GroupTargetModel {
    internal GroupTargetModel(TypeDeclModel decl, ImmutableArray<TargetFieldModel> fields) {
        TypeDecl = decl;
        TargetFields = fields.AsEquatableArray();
    }
    
    internal TypeDeclModel TypeDecl { get; }
    internal EquatableArray<TargetFieldModel> TargetFields { get; }
}
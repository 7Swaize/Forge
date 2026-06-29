using System;
using Forge.Annotations;
using Xunit;

namespace Forge.Tests.Features.AutoProperty.GeneratorTests;

public class AutoPropertyPermutations : TheoryData<Visibility, Accessors, ReturnMode> {
    public AutoPropertyPermutations() {
        foreach (var visibility in Enum.GetValues<Visibility>()) {
            foreach (var accessors in Enum.GetValues<Accessors>()) {
                foreach (var returnMode in Enum.GetValues<ReturnMode>()) {
                    if (!IsValid(accessors, returnMode)) {
                        continue;
                    }

                    Add(visibility, accessors, returnMode);
                }
            }
        }
    }

    private static bool IsValid(Accessors accessors, ReturnMode returnMode) =>
        returnMode switch {
            ReturnMode.RefStruct or ReturnMode.RefReadonlyStruct
                => accessors is Accessors.Get,
            _ => true,
        };
}
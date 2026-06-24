using System;
using Forge.RoslynShared;

public static class WeakReferenceExtensions {
    extension<T>(WeakReference<T> weakReference) where T : class {
        public T GetTargetOrThrow() {
            return weakReference.TryGetTarget(out T? target)
                ? target 
                : ThrowHelpers.ThrowWeakReferenceCollected<T>();
        }
    }
}
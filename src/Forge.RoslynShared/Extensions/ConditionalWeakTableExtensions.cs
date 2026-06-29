using System.Runtime.CompilerServices;
using Forge.RoslynShared;

public static class ConditionalWeakTableExtensions {
    extension<TKey, TValue>(ConditionalWeakTable<TKey, TValue> self) 
        where TKey : class
        where TValue : class
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TValue GetOrThrow(TKey key) {
            return self.TryGetValue(key, out TValue? value)
                ? value
                : ThrowHelpers.ThrowConditionalWeakTableKeyCollected<TKey, TValue>();
        }
    }    
}
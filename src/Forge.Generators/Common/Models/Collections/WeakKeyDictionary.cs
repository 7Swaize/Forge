using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Forge.RoslynShared;

namespace Forge.Generators.Common.Models.Collections;

public sealed class WeakKeyDictionary<TKey, TTarget> where TKey : class {
    private readonly Dictionary<WeakKey<TKey>, TTarget> _innerDictionary;
    private readonly IEqualityComparer<TKey> _keyComparer;

    public WeakKeyDictionary(IEqualityComparer<TKey>? keyComparer = null) {
        _keyComparer = keyComparer ?? EqualityComparer<TKey>.Default;
        _innerDictionary = new Dictionary<WeakKey<TKey>, TTarget>(new WeakKeyComparer<TKey>());
    }
    
    public TTarget this[TKey key] {
        get => GetValueOrThrow(key);
        set {
            WeakKey<TKey> lookupKey = new WeakKey<TKey>(key, _keyComparer);
            _innerDictionary[lookupKey] = value;
        }
    }
    
    public void Add(TKey key, TTarget value) {
        WeakKey<TKey> weakKey = new WeakKey<TKey>(key, _keyComparer);
        _innerDictionary.Add(weakKey, value);
    }
    
    public bool Remove(TKey key) {
        WeakKey<TKey> lookupKey = new WeakKey<TKey>(key, _keyComparer);
        return _innerDictionary.Remove(lookupKey);
    }
    
    public bool ContainsKey(TKey key) {
        WeakKey<TKey> lookupKey = new WeakKey<TKey>(key, _keyComparer);
        return _innerDictionary.ContainsKey(lookupKey);
    }

    public bool TryGetValue(TKey key, [NotNullWhen(true)] out TTarget? value) {
        WeakKey<TKey> lookupKey = new WeakKey<TKey>(key, _keyComparer);
        return _innerDictionary.TryGetValue(lookupKey, out value);
    }
    
    public TTarget GetValueOrThrow(TKey key) {
        WeakKey<TKey> lookupKey = new WeakKey<TKey>(key, _keyComparer);
        if (_innerDictionary.TryGetValue(lookupKey, out TTarget? value)) {
            return value;
        }
        
        return ThrowHelpers.KeyNotFoundException<TTarget>(key);
    }
}


public readonly struct WeakKey<TKey> : IEquatable<WeakKey<TKey>> where TKey : class {
    private readonly WeakReference<TKey> _weakRef;
    private readonly int _hashCode;
    private readonly IEqualityComparer<TKey> _comparer;

    public WeakKey(TKey key, IEqualityComparer<TKey>? comparer = null) {
        _weakRef = new WeakReference<TKey>(key);
        _comparer = comparer ?? EqualityComparer<TKey>.Default;
        _hashCode = _comparer.GetHashCode(key);
    }
    
    public bool TryGetTarget([NotNullWhen(true)] out TKey? target) {
        return _weakRef.TryGetTarget(out target);
    }

    public TKey GetTargetOrThrow() {
        return _weakRef.GetTargetOrThrow();
    }
    
    public override int GetHashCode() => _hashCode;

    public bool Equals(WeakKey<TKey> other) {
        if (_hashCode != other._hashCode) {
            return false;
        }

        bool standardThis = TryGetTarget(out TKey? thisTarget);
        bool standardOther = other.TryGetTarget(out TKey? otherTarget);

        if (standardThis && standardOther) {
            return _comparer.Equals(thisTarget!, otherTarget!);
        }
        
        return false;
    }
    
    public override bool Equals([NotNullWhen(true)] object? obj) {
        return obj is WeakKey<TKey> other && Equals(other);
    }
    
    public static bool operator ==(WeakKey<TKey> left, WeakKey<TKey> right) => left.Equals(right);
    public static bool operator !=(WeakKey<TKey> left, WeakKey<TKey> right) => !left.Equals(right);
}


public sealed class WeakKeyComparer<TKey> : IEqualityComparer<WeakKey<TKey>> where TKey : class {
    public bool Equals(WeakKey<TKey> x, WeakKey<TKey> y) => x.Equals(y);
    public int GetHashCode(WeakKey<TKey> obj) => obj.GetHashCode();
}
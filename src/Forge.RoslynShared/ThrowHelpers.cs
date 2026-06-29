using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Forge.RoslynShared;

public static class ThrowHelpers {
    [DoesNotReturn]
    public static TResult ThrowUnhandledBranch<TResult>(object value) =>
        throw new InvalidOperationException($"Unhandled value '{value}' in switch statement.");
    
    [DoesNotReturn]
    public static void ThrowUnhandledBranch(object value) =>
        throw new InvalidOperationException($"Unhandled value '{value}' in switch statement.");
    
    [DoesNotReturn]
    public static void ThrowUnreachable() =>
        throw new UnreachableException("Code should not be reachable.");
    
    [DoesNotReturn]
    public static TReturn ThrowWeakReferenceCollected<TReturn>() where TReturn : class =>
        throw new ObjectDisposedException(typeof(TReturn).Name, "The weak ref has been collected by the GC.");
    
    [DoesNotReturn]
    public static TReturn ThrowConditionalWeakTableKeyCollected<TKey, TReturn>()
        where TKey : class
        where TReturn : class =>
        throw new ObjectDisposedException(typeof(TKey).Name, "The weak ref has been collected by the GC.");

    [DoesNotReturn]
    public static TResult KeyNotFoundException<TResult>(object key) =>
        throw new KeyNotFoundException($"The given key {key} was not present in the dictionary.");
}


public sealed class UnreachableException(string message) : Exception(message);
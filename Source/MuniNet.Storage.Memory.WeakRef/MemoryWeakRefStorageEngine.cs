using BernhardHaus.Collections.WeakDictionary;
using MuniNet.Core.Hashing;
using MuniNet.Core.Storage;

namespace MuniNet.Storage.Memory.WeakRef;

internal class CacheKey : IEquatable<CacheKey>
{
    public CacheKey(FunctionHash functionHash, ReadOnlyMemory<byte> input)
    {
        FunctionHash = functionHash;
        Input = input;
    }

    public FunctionHash FunctionHash { get; }
    public ReadOnlyMemory<byte> Input { get; }

    public bool Equals(CacheKey? other)
    {
        if (other is null)
            return false;

        if (!FunctionHash.Equals(other.FunctionHash))
            return false;

        var eq = Input.Span.SequenceEqual(other.Input.Span);
        return eq;
    }

    public override int GetHashCode()
    {
        var hashCalc = new HashCode();
        hashCalc.Add(FunctionHash);
        hashCalc.AddBytes(Input.Span);

        var hash = hashCalc.ToHashCode();

        return hash;
    }

    public override bool Equals(object? obj)
    {
        return obj is CacheKey cacheKey && Equals(cacheKey);
    }
}

internal class CacheValue
{
    public CacheValue(ReadOnlyMemory<byte> output)
    {
        Output = output;
    }

    public ReadOnlyMemory<byte> Output { get; }
}

public class MemoryWeakRefStorageEngine : ICacheStorageEngineSelfManaged
{
    private readonly WeakDictionary<CacheKey, CacheValue> _cache;
    //private readonly Dictionary<CacheKey, CacheValue> _cache;
    private readonly object _lock = new();

    public MemoryWeakRefStorageEngine()
    {
        _cache = new();
    }

    public ValueTask<ReadOnlyMemory<byte>?> LookupCachedValue(FunctionHash functionHash, ReadOnlySpan<byte> inputValue)
    {
        var newKey = new CacheKey(functionHash, inputValue.ToArray());

        ReadOnlyMemory<byte>? result;
        lock (_lock)
        {
            if (_cache.Count > 0)
            {
                Math.Abs(0);
            }

            if (_cache.TryGetValue(newKey, out var value))
            {
                result = value.Output;
            }
            else
            {
                result = null;
            }
        }

        return ValueTask.FromResult(result);
    }

    public ValueTask<bool> TryAdd(FunctionHash functionHash, ReadOnlySpan<byte> inputValue, ReadOnlySpan<byte> outputValue)
    {
        var newKey = new CacheKey(functionHash, inputValue.ToArray());

        lock (_lock)
        {
            var result = _cache.TryAdd(newKey, new CacheValue(outputValue.ToArray()));

            //if (_cache.TryGetValue(newKey, out var found))
            //{
            //    Math.Abs(0);
            //}

            return ValueTask.FromResult(result);
        }
    }
}

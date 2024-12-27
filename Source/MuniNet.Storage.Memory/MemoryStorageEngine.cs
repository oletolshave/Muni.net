using MuniNet.Core.Hashing;
using MuniNet.Core.Storage;

namespace MuniNet.Storage.Memory;

internal struct CacheElement
{
    public CacheElement(FunctionHash functionHash, ReadOnlyMemory<byte> input, ReadOnlyMemory<byte> output)
    {
        FunctionHash = functionHash;
        Input = input;
        Output = output;
    }

    public FunctionHash FunctionHash { get; }
    public ReadOnlyMemory<byte> Input { get; }
    public ReadOnlyMemory<byte> Output { get; }
}

internal class MemoryStorageEngine : ICacheStorageEngine
{
    private readonly List<CacheElement> _simpleList = new();
    private readonly object _lock = new object();

    public ValueTask<ReadOnlyMemory<byte>?> LookupCachedValue(
        FunctionHash functionHash, ReadOnlySpan<byte> inputValue)
    {
        ReadOnlyMemory<byte>? result;

        lock (_lock)
        {
            var index = FindIndexWhileLocked(functionHash, inputValue);
            if (index == -1)
                result = null;
            else
                result = _simpleList[index].Output;
        }

        return ValueTask.FromResult(result);
    }

    public ValueTask<bool> TryAdd(FunctionHash functionHash, ReadOnlySpan<byte> inputValue,
        ReadOnlySpan<byte> outputValue)
    {
        bool added;
        lock (_lock)
        {
            var index = FindIndexWhileLocked(functionHash, inputValue);
            if (index != -1)
                added = false;
            else
            {
                _simpleList.Add(new CacheElement(functionHash,
                    inputValue.ToArray(),
                    outputValue.ToArray()));

                added = true;
            }
        }

        return ValueTask.FromResult(added);
    }

    private int FindIndexWhileLocked(FunctionHash functionHash, ReadOnlySpan<byte> inputValue)
    {
        for (var i = 0; i < _simpleList.Count; i++)
        {
            var element = _simpleList[i];
            if (!element.FunctionHash.Equals(functionHash))
                continue;

            if (!element.Input.Span.SequenceEqual(inputValue))
                continue;

            return i;
        }

        return -1;
    }
}

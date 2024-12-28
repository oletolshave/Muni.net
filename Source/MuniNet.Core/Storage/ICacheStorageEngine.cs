using MuniNet.Core.Hashing;

namespace MuniNet.Core.Storage;

public interface ICacheStorageEngine
{
    ValueTask<ReadOnlyMemory<byte>?> LookupCachedValue(
        FunctionHash functionHash, ReadOnlySpan<byte> inputValue);

    ValueTask<bool> TryAdd(FunctionHash functionHash,
        ReadOnlySpan<byte> inputValue,
        ReadOnlySpan<byte> outputValue);

    ValueTask<long> ReadEstimatedCacheSize();
}

namespace MuniNet.Core.Storage;

// A storage engine that by default keeps everything. The data in the cache needs to be managed explicitly,
// to not grow beyond all limits.
public interface ICacheStorageEngineClassic : ICacheStorageEngineBase
{

    //IAsyncEnumerable<KeyValuePair<FunctionHash, ReadOnlyMemory<byte>>> ListKeys();

    //ValueTask<bool> TryRemove(FunctionHash functionHas, ReadOnlySpan<byte> inputValue);
    ValueTask RemoveAllKeys();

    ValueTask<long> ReadEstimatedCacheSize();
}
using MuniNet.Core.Hashing;
using MuniNet.Core.Storage;
using System.IO.Abstractions;
using System.Runtime.Loader;

namespace MuniNet.Core.Caching;

public class GenericCacheManagerClassic : GenericCacheManagerBase, ICacheManagerControl
{
    private readonly ICacheStorageEngineClassic _storageEngineClassic;
    private readonly long _maxCacheSizeBytes;

    public GenericCacheManagerClassic(
        IFileSystem fileSystem,
        AssemblyLoadContext assemblyLoadContext,
        ICacheStorageEngineClassic storageEngineClassic,
        long maxCacheSizeBytes)
        : base(fileSystem, assemblyLoadContext, storageEngineClassic)
    {
        _storageEngineClassic = storageEngineClassic;
        _maxCacheSizeBytes = maxCacheSizeBytes;
    }

    
    public Task GarbageCollect(CancellationToken cancellationToken = default)
    {
        // TODO: Implement this

        return Task.CompletedTask;
    }

    protected override async Task EvictUntilEnoughRoomFor(
        FunctionHash functionHash, 
        ReadOnlyMemory<byte> inputValue,
        ReadOnlyMemory<byte> outputValue)
    {
        var additionalBytes = EstimateConsumedBytes(functionHash, inputValue.Span, outputValue.Span);

        // TODO: This is not optimal at all. Please optimize.
        var currentCacheSize = await _storageEngineClassic.ReadEstimatedCacheSize();

        if (currentCacheSize + additionalBytes > _maxCacheSizeBytes)
        {
            await _storageEngineClassic.RemoveAllKeys();
        }
    }
}

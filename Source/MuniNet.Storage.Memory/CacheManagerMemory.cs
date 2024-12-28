using MuniNet.Base;
using MuniNet.Core.Caching;
using System.IO.Abstractions;
using System.Runtime.Loader;

namespace MuniNet.Storage.Memory;

public class CacheManagerMemory : ICacheManager, ICacheManagerControl
{
    private readonly MemoryStorageEngine _storageEngine;
    private readonly GenericCacheManagerClassic _cacheManager;

    public CacheManagerMemory(
        IFileSystem fileSystem,
        AssemblyLoadContext assemblyLoadContext,
        long maxCacheSize)
    {
        _storageEngine = new();

        _cacheManager = new GenericCacheManagerClassic(fileSystem, assemblyLoadContext,
            _storageEngine,
            maxCacheSize);
    }

    public static CacheManagerMemory New(
        IFileSystem? fileSystem = null,
        AssemblyLoadContext? assemblyLoadContext = null,
        long? maxCacheBytes = null)
    {
        var memoryInfo = GC.GetGCMemoryInfo();
        if (maxCacheBytes is null)
        {
            // By default: 10% of memory available to .NET
            maxCacheBytes = (long) (memoryInfo.TotalAvailableMemoryBytes * 0.1);
        }

        return new CacheManagerMemory(
            fileSystem ?? new FileSystem(),
            assemblyLoadContext ?? AssemblyLoadContext.Default,
            maxCacheBytes.Value);
    }

    public ICacheCalculation<TOutput, TInput>
        For<TOutput, TInput>(CalculationAsync<TOutput, TInput> calc)
    {
        return _cacheManager.For<TOutput, TInput>(calc);
    }

    public Task GarbageCollect(CancellationToken cancellationToken = default)
    {
        return _cacheManager.GarbageCollect(cancellationToken);
    }
}


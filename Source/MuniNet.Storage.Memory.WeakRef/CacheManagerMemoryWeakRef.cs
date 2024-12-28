using MuniNet.Base;
using MuniNet.Core.Caching;
using System.IO.Abstractions;
using System.Runtime.Loader;

namespace MuniNet.Storage.Memory.WeakRef;

public class CacheManagerMemoryWeakRef : ICacheManager
{
    private readonly MemoryWeakRefStorageEngine _storageEngine;
    private readonly GenericCacheManagerSelfManaged _cacheManager;

    public CacheManagerMemoryWeakRef(
        IFileSystem fileSystem,
        AssemblyLoadContext assemblyLoadContext)
    {
        _storageEngine = new();

        _cacheManager = new GenericCacheManagerSelfManaged(fileSystem, assemblyLoadContext,
            _storageEngine);
    }

    public static CacheManagerMemoryWeakRef New(
        IFileSystem? fileSystem = null,
        AssemblyLoadContext? assemblyLoadContext = null)
    {
        return new CacheManagerMemoryWeakRef(
            fileSystem ?? new FileSystem(),
            assemblyLoadContext ?? AssemblyLoadContext.Default);
    }

    public ICacheCalculation<TOutput, TInput> For<TOutput, TInput>(CalculationAsync<TOutput, TInput> calc)
    {
        return _cacheManager.For(calc);
    }
}

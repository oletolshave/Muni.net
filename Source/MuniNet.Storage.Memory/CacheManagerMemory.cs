using MuniNet.Base;
using MuniNet.Core.Caching;
using System.IO.Abstractions;
using System.Runtime.Loader;

namespace MuniNet.Storage.Memory;

public class CacheManagerMemory : ICacheManager
{
    private readonly MemoryStorageEngine _storageEngine;
    private readonly GenericCacheManager _cacheManager;

    public CacheManagerMemory(
        IFileSystem fileSystem,
        AssemblyLoadContext assemblyLoadContext)
    {
        _storageEngine = new();

        _cacheManager = new GenericCacheManager(fileSystem, assemblyLoadContext,
            _storageEngine);
    }

    public static CacheManagerMemory New(
        IFileSystem? fileSystem = null,
        AssemblyLoadContext? assemblyLoadContext = null)
    {
        return new CacheManagerMemory(
            fileSystem ?? new FileSystem(),
            assemblyLoadContext ?? AssemblyLoadContext.Default);
    }

    public ICacheCalculation<TOutput, TInput>
        For<TOutput, TInput>(Calculation<TOutput, TInput> calc)
    {
        return _cacheManager.For<TOutput, TInput>(calc);
    }
}


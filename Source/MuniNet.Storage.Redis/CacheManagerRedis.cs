using System.IO.Abstractions;
using System.Runtime.Loader;

namespace MuniNet.Storage.Redis;

public class CacheManagerRedis
{
    private readonly RedisStorageEngine _storageEngine;

    public CacheManagerRedis(
            IFileSystem fileSystem,
            AssemblyLoadContext assemblyLoadContext)
    {
        _storageEngine = new();

        //_cacheManager = new GenericCacheManagerClassic(fileSystem, assemblyLoadContext,
        //    _storageEngine,
        //    maxCacheSize);
    }

    public static CacheManagerRedis New(
        IFileSystem? fileSystem = null,
        AssemblyLoadContext? assemblyLoadContext = null)
    {
        return new CacheManagerRedis(
            fileSystem ?? new FileSystem(),
            assemblyLoadContext ?? AssemblyLoadContext.Default);
    }
}

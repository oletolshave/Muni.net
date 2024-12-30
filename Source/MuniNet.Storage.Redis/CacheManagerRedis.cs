using MuniNet.Base;
using MuniNet.Core.Caching;
using System.IO.Abstractions;
using System.Runtime.Loader;

namespace MuniNet.Storage.Redis;

public class CacheManagerRedis : ICacheManager
{
    private readonly RedisStorageEngine _storageEngine;
    private readonly GenericCacheManagerSelfManaged _cacheManager;

    public CacheManagerRedis(
            IFileSystem fileSystem,
            AssemblyLoadContext assemblyLoadContext,
            string redisConfigurationName)
    {
        _storageEngine = new (redisConfigurationName);

        _cacheManager = new GenericCacheManagerSelfManaged(fileSystem, assemblyLoadContext,
            _storageEngine);
    }

    public static CacheManagerRedis New(
        IFileSystem? fileSystem = null,
        AssemblyLoadContext? assemblyLoadContext = null,
        string? redisConfigurationName = null)
    {
        return new CacheManagerRedis(
            fileSystem ?? new FileSystem(),
            assemblyLoadContext ?? AssemblyLoadContext.Default,
            redisConfigurationName: redisConfigurationName ?? "localhost");
    }

    public ICacheCalculation<TOutput, TInput> For<TOutput, TInput>(CalculationAsync<TOutput, TInput> calc)
    {
        return _cacheManager.For(calc);
    }
}

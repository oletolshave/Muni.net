using MuniNet.Base;
using MuniNet.StorageAgnostic.Tests;

namespace MuniNet.Storage.Redis.Tests;

//[Trait("Category", "RedisIntegration")]
public class StorageAgnosticTests : AllTests
{
    protected override ICacheManager NewCacheManager()
    {
        return CacheManagerRedis.New();
    }
}

using MuniNet.Base;
using MuniNet.StorageAgnostic.Tests.SelfManaged;

namespace MuniNet.Storage.Redis.Tests;

[Trait("Category", "RedisIntegration")]
public class StorageAgnosticTests : SelfManaged_AllTests
{
    protected override ICacheManager NewCacheManager()
    {
        return CacheManagerRedis.New();
    }
}

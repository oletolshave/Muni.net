using MuniNet.Base;
using MuniNet.Storage.Memory;
using MuniNet.StorageAgnostic.Tests;

namespace FirstTest.Tests;

[Trait("Category", "CI")]
public class TestWithDefaults : AllTests
{
    protected override ICacheManager NewCacheManager()
    {
        return CacheManagerMemory.New();
    }
}

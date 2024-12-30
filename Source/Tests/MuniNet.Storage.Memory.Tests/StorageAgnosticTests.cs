using MuniNet.Base;
using MuniNet.StorageAgnostic.Tests.Classic;

namespace MuniNet.Storage.Memory.Tests;

[Trait("Category", "CI")]
public class StorageAgnosticTests : Classic_AllTests
{
    protected override ICacheManager NewCacheManager()
    {
        return CacheManagerMemory.New();
    }
}

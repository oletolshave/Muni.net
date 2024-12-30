using MuniNet.Base;
using MuniNet.StorageAgnostic.Tests;

namespace MuniNet.Storage.Memory.WeakRef.Tests;

[Trait("Category", "CI")]
public class StorageAgnosticTests : AllTests
{
    protected override ICacheManager NewCacheManager()
    {
        return CacheManagerMemoryWeakRef.New();
    }
}

using MuniNet.Base;
using MuniNet.StorageAgnostic.Tests.SelfManaged;

namespace MuniNet.Storage.Memory.WeakRef.Tests;

[Trait("Category", "CI")]
public class StorageAgnosticTests : SelfManaged_AllTests
{
    protected override ICacheManager NewCacheManager()
    {
        return CacheManagerMemoryWeakRef.New();
    }
}

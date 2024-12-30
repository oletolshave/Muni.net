using MuniNet.Base;

namespace MuniNet.StorageAgnostic.Tests;

public abstract partial class AllTests
{
    protected abstract ICacheManager NewCacheManager();
}

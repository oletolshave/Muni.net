using MuniNet.Base;

namespace MuniNet.StorageAgnostic.Tests.Common;

public abstract partial class AllTests
{
    protected abstract ICacheManager NewCacheManager();
}

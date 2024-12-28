namespace MuniNet.Core.Caching;

public interface ICacheManagerControl
{
    Task GarbageCollect(CancellationToken cancellationToken = default);
}

using MuniNet.Storage.Redis;

namespace Storage.Redis.FirstTest.Tests;

public class UnitTest1
{
    [Fact(Skip = "Does not yet work.")]
    public void ItCanConstructTheRedisClient()
    {
        var sut = CacheManagerRedis.New();
    }
}
using MuniNet.Storage.Redis;

namespace Storage.Redis.FirstTest.Tests;

[Trait("Category", "RedisIntegration")]
public class UnitTest1
{
    [Fact(Skip = "Does not yet work.")]
    //[Fact]
    public void ItCanConstructTheRedisClient()
    {
        var sut = CacheManagerRedis.New();
    }

}

using AutoFixture.Xunit2;
using FirstTestLib;
using FluentAssertions;
using MuniNet.Storage.Redis;

namespace Storage.Redis.FirstTest.Tests;

[Trait("Category", "RedisIntegration")]
public class UnitTest1
{
    [Theory]
    [InlineAutoData(1000, 2)]
    public async Task ItDoesNotGrowTheCacheBeyondAllLimits(
        int calculationCount, int rounds)
    {
        var sut = CacheManagerRedis.New();

        var plusOne = new PlusOne();
        var plusOneCalc = sut.For(plusOne);

        for (var round = 0; round < rounds; round++)
        {
            for (long i = 0; i < calculationCount; i++)
            {
                var plusOneResult = await plusOneCalc.CalculateAsync(i);
                plusOneResult.Should().Be(i + 1);
            }

            await Task.Delay(1000);
        }

        // It should not have been possible to keep all "calculationCount" results in the cache.
        plusOne.CallCount.Should().BeGreaterThan(calculationCount);
    }
}

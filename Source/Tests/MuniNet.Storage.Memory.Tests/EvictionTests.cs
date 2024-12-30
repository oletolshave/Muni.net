using AutoFixture.Xunit2;
using FluentAssertions;
using SimpleTestLib;

namespace MuniNet.Storage.Memory.Tests;

[Trait("Category", "CI")]
public class EvictionTests
{
    [Theory]
    [InlineAutoData(1024, 1000, 2)]
    public async Task ItDoesNotGrowTheCacheBeyondAllLimits(
        long maxCacheBytes, int calculationCount, int rounds)
    {
        var sut = CacheManagerMemory.New(maxCacheBytes: maxCacheBytes);

        var plusOne = new PlusOne();
        var plusOneCalc = sut.For(plusOne);

        for(var round=0; round < rounds; round++)
        {
            for (long i=0; i< calculationCount; i++)
            {
                var plusOneResult = await plusOneCalc.CalculateAsync(i);
                plusOneResult.Should().Be(i + 1);
            }
        }

        // It should not have been possible to keep all "calculationCount" results in the cache.
        plusOne.CallCount.Should().BeGreaterThan(calculationCount);
    }
}

using FluentAssertions;
using SimpleTestLib;
using Xunit;

namespace MuniNet.StorageAgnostic.Tests.Common;

public partial class AllTests
{
    [Fact]
    public async Task ItCanCacheBinaryData()
    {
        var sut = NewCacheManager();

        var calc = sut.For(new CalcSimpleBinaryData());

        var result = await calc.CalculateAsync(3);

        var sum = result.Data.Select(b => (int)b).Sum();

        result.Data.Length.Should().Be(3);
        sum.Should().Be(3);
    }

    [Fact]
    public async Task ItCanCacheAndReuseBinaryData()
    {
        var sut = NewCacheManager();

        var calc = sut.For(new CalcSimpleBinaryData());

        var result1 = await calc.CalculateAsync(4);
        var result2 = await calc.CalculateAsync(4);

        var sum1 = result1.Data.Select(b => (int)b).Sum();
        var sum2 = result2.Data.Select(b => (int)b).Sum();

        result1.Data.Length.Should().Be(4);
        sum1.Should().Be(6);

        result2.Data.Length.Should().Be(4);
        sum2.Should().Be(6);
    }
}

using FluentAssertions;
using SimpleTestLib;
using Xunit;

namespace MuniNet.StorageAgnostic.Tests.Common;

public partial class AllTests
{
    [Fact]
    public async Task ItWorksEvenWhenPotentiallyCachingAVeryLargeAmountOfData()
    {
        var sut = NewCacheManager();

        var calc = sut.For(new CalcWithLargeBinaryData());

        // Each repetition caches a megabyte potentially
        for (int i=0; i<256; i++)
        {
            var result = await calc.CalculateAsync(i);
            result.IntValue.Should().Be(i);
            result.Payload.Length.Should().Be(CalcWithLargeBinaryData.PayloadSize);
        }
    }
}

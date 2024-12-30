using AutoFixture.Xunit2;
using FluentAssertions;
using SimpleTestLib;
using Xunit;

namespace MuniNet.StorageAgnostic.Tests.Classic;

public partial class Classic_AllTests
{
    [Theory]
    [InlineAutoData(0, 1)]
    [InlineAutoData(1, 1)]
    [InlineAutoData(6, 7)]
    [InlineAutoData(8, 9)]
    public async Task ItOnlyCalculatesEachFibonacciNumberOnce(int nthNumber, int expectedCalls)
    {
        var sut = NewCacheManager();

        var fibCalc = new FibonacciCalcMemo(sut);
        var calc = sut.For(fibCalc);

        await calc.CalculateAsync(nthNumber);

        fibCalc.CallCount.Should().Be(expectedCalls);
    }
}

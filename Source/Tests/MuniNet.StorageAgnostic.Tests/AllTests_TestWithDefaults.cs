using AutoFixture.Xunit2;
using FluentAssertions;
using SimpleTestLib;
using Xunit;

namespace MuniNet.StorageAgnostic.Tests;

public partial class AllTests
{
    [Theory]
    [InlineAutoData(0, 0)]
    [InlineAutoData(1, 1)]
    [InlineAutoData(6, 8)]
    [InlineAutoData(8, 21)]
    public async Task ItReturnsCorrectResultsOnFirstCalculations(int nthNumber, int expectedResult)
    {
        var sut = NewCacheManager();

        var fibCalc = new FibonacciCalc();
        var actualResult = await sut.For(fibCalc).CalculateAsync(nthNumber);

        actualResult.Should().Be(expectedResult);
        fibCalc.CallCount.Should().Be(1);
    }
}

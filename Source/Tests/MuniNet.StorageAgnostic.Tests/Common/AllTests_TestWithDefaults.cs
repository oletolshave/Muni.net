using AutoFixture.Xunit2;
using FluentAssertions;
using SimpleTestLib;
using Xunit;

namespace MuniNet.StorageAgnostic.Tests.Common;

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
        //fibCalc.CallCount.Should().Be(1); // Does not necessarily hold for SelfManaged caches.
    }


    [Theory]
    [InlineAutoData(0, 0)]
    [InlineAutoData(1, 1)]
    [InlineAutoData(6, 8)]
    [InlineAutoData(8, 21)]
    public async Task ItReturnsCorrectResultsOnMultipleCalculations(int nthNumber, int expectedResult)
    {
        var sut = NewCacheManager();


        var fibCalc = new FibonacciCalc();
        var calc = sut.For(fibCalc);

        var actualResult1 = await calc.CalculateAsync(nthNumber);
        var actualResult2 = await calc.CalculateAsync(nthNumber);
        var actualResult3 = await calc.CalculateAsync(nthNumber);

        actualResult1.Should().Be(expectedResult);
        actualResult2.Should().Be(expectedResult);
        actualResult3.Should().Be(expectedResult);
    }
}

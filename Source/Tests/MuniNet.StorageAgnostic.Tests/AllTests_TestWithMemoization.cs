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
    public async Task ItReturnsCorrectMemoizedResultsOnMultipleCalculations(int nthNumber, int expectedResult)
    {
        var sut = NewCacheManager();


        var fibCalc = new FibonacciCalcMemo(sut);
        var calc = sut.For(fibCalc);

        var actualResult1 = await calc.CalculateAsync(nthNumber);
        var actualResult2 = await calc.CalculateAsync(nthNumber);
        var actualResult3 = await calc.CalculateAsync(nthNumber);

        actualResult1.Should().Be(expectedResult);
        actualResult2.Should().Be(expectedResult);
        actualResult3.Should().Be(expectedResult);
    }

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

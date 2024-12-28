using AutoFixture.Xunit2;
using FluentAssertions;
using MuniNet.Storage.Memory;

namespace FirstTest.Tests;

[Trait("Category", "CI")]
public class TestWithDefaults
{
    [Theory]
    [InlineAutoData(0, 0)]
    [InlineAutoData(1, 1)]
    [InlineAutoData(6, 8)]
    [InlineAutoData(8, 21)]
    public async Task ItReturnsCorrectResultsOnFirstCalculations(int nthNumber, int expectedResult)
    {
        var sut = CacheManagerMemory.New();

        var fibCalc = new FibonacciCalc();
        var actualResult = await sut.For(fibCalc).Calculate(nthNumber);

        actualResult.Should().Be(expectedResult);
        fibCalc.CallCount.Should().Be(1);
    }

    [Theory]
    [InlineAutoData(0, 0)]
    [InlineAutoData(1, 1)]
    [InlineAutoData(6, 8)]
    [InlineAutoData(8, 21)]
    public async Task ItReturnsCorrectResultsOnMultipleCalculations(int nthNumber, int expectedResult)
    {
        var sut = CacheManagerMemory.New();


        var fibCalc = new FibonacciCalc();
        var calc = sut.For(fibCalc);

        var actualResult1 = await calc.Calculate(nthNumber);
        var actualResult2 = await calc.Calculate(nthNumber);
        var actualResult3 = await calc.Calculate(nthNumber);

        actualResult1.Should().Be(expectedResult);
        actualResult2.Should().Be(expectedResult);
        actualResult3.Should().Be(expectedResult);
    }

    [Theory]
    [InlineAutoData(0)]
    [InlineAutoData(1)]
    [InlineAutoData(6)]
    [InlineAutoData(8)]
    public async Task ItOnlyCalculatesOnce(int nthNumber)
    {
        var sut = CacheManagerMemory.New();

        var fibCalc = new FibonacciCalc();
        var calc = sut.For(fibCalc);

        await calc.Calculate(nthNumber);
        await calc.Calculate(nthNumber);
        await calc.Calculate(nthNumber);

        fibCalc.CallCount.Should().Be(1);
    }

    [Theory]
    [InlineAutoData(0)]
    [InlineAutoData(1)]
    [InlineAutoData(6)]
    [InlineAutoData(8)]
    public async Task ItOnlyCalculatesOnceEvenWhenForIsReused(int nthNumber)
    {
        var sut = CacheManagerMemory.New();

        var fibCalc1 = new FibonacciCalc();
        var fibCalc2 = new FibonacciCalc();
        var fibCalc3 = new FibonacciCalc();

        await sut.For(fibCalc1).Calculate(nthNumber);
        await sut.For(fibCalc2).Calculate(nthNumber);
        await sut.For(fibCalc3).Calculate(nthNumber);

        var totalCallCalc = fibCalc1.CallCount
            + fibCalc2.CallCount
            + fibCalc3.CallCount;

        totalCallCalc.Should().Be(1);
    }

    [Theory]
    [InlineAutoData(5, 8)]
    public async Task ItHandlesMultipleCachedValues(int first, int second)
    {
        var sut = CacheManagerMemory.New();

        var fibCalcInner = new FibonacciCalc();
        var fibCalc = sut.For(fibCalcInner);

        var result1 = await fibCalc.Calculate(first);
        var result2 = await fibCalc.Calculate(second);

        var result3 = await fibCalc.Calculate(first);
        var result4 = await fibCalc.Calculate(second);

        result3.Should().Be(result1);
        result4.Should().Be(result2);
        fibCalcInner.CallCount.Should().Be(2);
    }
}

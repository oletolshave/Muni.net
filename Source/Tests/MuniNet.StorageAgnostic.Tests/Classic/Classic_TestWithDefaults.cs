using AutoFixture.Xunit2;
using FluentAssertions;
using SimpleTestLib;
using Xunit;

namespace MuniNet.StorageAgnostic.Tests.Classic;

public partial class Classic_AllTests
{
    [Theory]
    [InlineAutoData(0)]
    [InlineAutoData(1)]
    [InlineAutoData(6)]
    [InlineAutoData(8)]
    public async Task ItOnlyCalculatesOnce(int nthNumber)
    {
        var sut = NewCacheManager();

        var fibCalc = new FibonacciCalc();
        var calc = sut.For(fibCalc);

        await calc.CalculateAsync(nthNumber);
        await calc.CalculateAsync(nthNumber);
        await calc.CalculateAsync(nthNumber);

        fibCalc.CallCount.Should().Be(1);
    }

    [Theory]
    [InlineAutoData(0)]
    [InlineAutoData(1)]
    [InlineAutoData(6)]
    [InlineAutoData(8)]
    public async Task ItOnlyCalculatesOnceEvenWhenForIsReused(int nthNumber)
    {
        var sut = NewCacheManager();

        var fibCalc1 = new FibonacciCalc();
        var fibCalc2 = new FibonacciCalc();
        var fibCalc3 = new FibonacciCalc();

        await sut.For(fibCalc1).CalculateAsync(nthNumber);
        await sut.For(fibCalc2).CalculateAsync(nthNumber);
        await sut.For(fibCalc3).CalculateAsync(nthNumber);

        var totalCallCalc = fibCalc1.CallCount
            + fibCalc2.CallCount
            + fibCalc3.CallCount;

        totalCallCalc.Should().Be(1);
    }

    [Theory]
    [InlineAutoData(5, 8)]
    public async Task ItHandlesMultipleCachedValues(int first, int second)
    {
        var sut = NewCacheManager();

        var fibCalcInner = new FibonacciCalc();
        var fibCalc = sut.For(fibCalcInner);

        var result1 = await fibCalc.CalculateAsync(first);
        var result2 = await fibCalc.CalculateAsync(second);

        var result3 = await fibCalc.CalculateAsync(first);
        var result4 = await fibCalc.CalculateAsync(second);

        result3.Should().Be(result1);
        result4.Should().Be(result2);
        fibCalcInner.CallCount.Should().Be(2);
    }
}

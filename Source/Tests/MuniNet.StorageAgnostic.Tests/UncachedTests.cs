using AutoFixture.Xunit2;
using FluentAssertions;
using SimpleTestLib;
using Xunit;

namespace MuniNet.StorageAgnostic.Tests;

[Trait("Category", "CI")]
public class UncachedTests
{
    [Theory]
    [InlineAutoData(0, 0)]
    [InlineAutoData(1, 1)]
    [InlineAutoData(2, 1)]
    [InlineAutoData(3, 2)]
    [InlineAutoData(4, 3)]
    [InlineAutoData(5, 5)]
    [InlineAutoData(6, 8)]
    [InlineAutoData(7, 13)]
    [InlineAutoData(8, 21)]
    public async Task ItCalculatesSomeCorrectFibonacciNumbers(int nthNumber, int expectedResult)
    {
        var sut = new FibonacciCalc();

        var actualResult = await sut.CalculateAsync(nthNumber);

        actualResult.Should().Be(expectedResult);
    }
}

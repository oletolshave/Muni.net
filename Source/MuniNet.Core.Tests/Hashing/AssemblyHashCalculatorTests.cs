using FluentAssertions;
using System.IO.Abstractions;
using System.Runtime.Loader;

namespace MuniNet.Core.Hashing.Tests;

[Trait("Category", "CI")]
public class AssemblyHashCalculatorTests
{
    [Fact]
    public void ItCanGenerateAHashValue()
    {
        var sut = MakeDefaultCalculator();
        sut.CalculateAssemblyHash(typeof(AssemblyHashCalculatorTests).Assembly);
    }

    [Fact]
    public void ItGeneratesTheSameHashValueForTheSameAssembly()
    {
        var sut = MakeDefaultCalculator();
        var hash1 = sut.CalculateAssemblyHash(typeof(AssemblyHashCalculatorTests).Assembly);
        var hash2 = sut.CalculateAssemblyHash(typeof(AssemblyHashCalculatorTests).Assembly);

        hash1.Equals(hash2).Should().BeTrue();
    }

    [Fact]
    public void ItGeneratesTheSameHashValueForTheSameAssemblyEvenIfCalculatorsAreDifferent()
    {
        var sut1 = MakeDefaultCalculator();
        var sut2 = MakeDefaultCalculator();

        var hash1 = sut1.CalculateAssemblyHash(typeof(AssemblyHashCalculatorTests).Assembly);
        var hash2 = sut2.CalculateAssemblyHash(typeof(AssemblyHashCalculatorTests).Assembly);

        hash1.Equals(hash2).Should().BeTrue();
    }

    [Fact]
    public void ItGeneratesDifferentHashValueForDifferentAssemblies()
    {
        var sut = MakeDefaultCalculator();
        var hash1 = sut.CalculateAssemblyHash(typeof(AssemblyHashCalculatorTests).Assembly);
        var hash2 = sut.CalculateAssemblyHash(typeof(object).Assembly);

        hash1.Equals(hash2).Should().BeFalse();
    }

    public AssemblyHashCalculator MakeDefaultCalculator()
    {
        return new AssemblyHashCalculator(
            new FileSystem(),
            AssemblyLoadContext.Default);
    }
}

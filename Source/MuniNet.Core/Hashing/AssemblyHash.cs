using System.Diagnostics.CodeAnalysis;

namespace MuniNet.Core.Hashing;

public struct AssemblyHash : IEquatable<AssemblyHash>
{
    private readonly HashIdent _algorithm;
    
    // Only null when default constructor is used.
    private readonly ReadOnlyMemory<byte>? _hashValue;  

    public AssemblyHash(HashIdent algorithm, ReadOnlyMemory<byte> hashValue)
    {
        _algorithm = algorithm;
        _hashValue = hashValue;
    }

    public HashIdent Algorithm => _algorithm;
    public ReadOnlyMemory<byte> HashValue => _hashValue is null
        ? (byte[]) []
        : _hashValue.Value;

    public bool Equals(AssemblyHash other)
    {
        return HashValue.Span.SequenceEqual(other.HashValue.Span)
            && _algorithm == other._algorithm;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is AssemblyHash other && Equals(other);
    }

    public override int GetHashCode()
    {
        var hashCode = new HashCode();

        hashCode.AddBytes(HashValue.Span);

        return hashCode.ToHashCode();
    }

    public override string ToString()
    {
        if (_algorithm == HashIdent.MD5)
            return $"MD5:{Convert.ToBase64String(HashValue.Span)}";

        return "(Unknown)";
    }

    public static bool operator ==(AssemblyHash x, AssemblyHash y)
    {
        return x.Equals(y);
    }

    public static bool operator !=(AssemblyHash x, AssemblyHash y)
    {
        return !x.Equals(y);
    }
}

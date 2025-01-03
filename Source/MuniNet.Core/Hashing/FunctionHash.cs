﻿using System.Diagnostics.CodeAnalysis;

namespace MuniNet.Core.Hashing;

public struct FunctionHash : IEquatable<FunctionHash>
{
    private readonly HashIdent _algorithm;
    private readonly ReadOnlyMemory<byte>? _hashValue;
    private int? _hashCode;
    private string? _toString;

    public FunctionHash(HashIdent algorithm, ReadOnlyMemory<byte> hashValue)
    {
        _algorithm = algorithm;
        _hashValue = hashValue;
    }

    public HashIdent Algorithm => _algorithm;
    public ReadOnlyMemory<byte> HashValue => _hashValue is null
        ? (byte[])[]
        : _hashValue.Value;

    public bool Equals(FunctionHash other)
    {
        return HashValue.Span.SequenceEqual(other.HashValue.Span)
            && _algorithm == other._algorithm;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is FunctionHash other && Equals(other);
    }

    public override int GetHashCode()
    {
        if (_hashCode is not null)
            return _hashCode.Value;

        var hashCode = new HashCode();
        hashCode.AddBytes(HashValue.Span);

        _hashCode = hashCode.ToHashCode();

        return _hashCode.Value;
    }

    public override string ToString()
    {
        if (_toString is null)
        {
            if (_algorithm == HashIdent.MD5)
                _toString = $"MD5:{Convert.ToBase64String(HashValue.Span)}";
            else
                _toString = "(Unknown)";
        }

        return _toString;
    }

    public static bool operator ==(FunctionHash x, FunctionHash y)
    {
        return x.Equals(y);
    }

    public static bool operator !=(FunctionHash x, FunctionHash y)
    {
        return !x.Equals(y);
    }

    public static FunctionHash CalculateFunctionHash(AssemblyHash assemblyHash,
        string calcTypeFullName)
    {
        var hashCalcContext = new HashCalculationContext();
        hashCalcContext.FeedBytes(assemblyHash.HashValue.Span);
        hashCalcContext.FeedBytes(System.Text.Encoding.UTF8.GetBytes(calcTypeFullName));

        var hash = hashCalcContext.GetFinalHashBytes();
        if (hash is null)
            throw new InvalidOperationException("No hash could be generated.");

        return new FunctionHash(hash.Value.algorithm, hash.Value.hashValue);
    }
}

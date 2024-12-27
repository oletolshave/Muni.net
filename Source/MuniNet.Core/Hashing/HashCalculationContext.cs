using System.Security.Cryptography;

namespace MuniNet.Core.Hashing;

internal class HashCalculationContext
{
    private static readonly HashAlgorithmName HashAlgorithmName = HashAlgorithmName.MD5;

    private readonly IncrementalHash _hashAlgorithm;

    public HashCalculationContext()
    {
        _hashAlgorithm = IncrementalHash.CreateHash(HashAlgorithmName);
    }

    public string? GetFinalHash()
    {
        var result = GetFinalHashBytes();
        if (result is null)
            return null;

        return Convert.ToBase64String(result.Value.hashValue);
    }

    public (HashIdent algorithm, byte[] hashValue)? GetFinalHashBytes()
    {
        var output = new byte[_hashAlgorithm.HashLengthInBytes];
        if (!_hashAlgorithm.TryGetHashAndReset(output, out var bytesWritten))
            return null;

        if (bytesWritten <= 0)
            return null;

        return (HashIdent.MD5, output);
    }

    public HashCalculationContext FeedBytes(ReadOnlySpan<byte> buffer)
    {
        _hashAlgorithm.AppendData(buffer);

        return this;
    }

    public HashCalculationContext FeedBytes(byte[] buffer, int offset, int count)
    {
        return FeedBytes(buffer.AsSpan(offset, count));
    }
}

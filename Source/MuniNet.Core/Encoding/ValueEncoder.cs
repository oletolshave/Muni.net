namespace MuniNet.Core.Encoding;

public abstract class ValueEncoder<T>
{
    public abstract ReadOnlyMemory<byte> Encode(T value);

    public abstract T Decode(ReadOnlySpan<byte> value);
}

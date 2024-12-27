using ProtoBuf;

namespace MuniNet.Core.Encoding;

public class ProtobufValueEncoder<T> : ValueEncoder<T>
{
    public override T Decode(ReadOnlySpan<byte> value)
    {
        return Serializer.Deserialize<T>(value);
    }

    public override ReadOnlyMemory<byte> Encode(T value)
    {
        using var stream = new MemoryStream();
        Serializer.Serialize(stream, value);

        var bytes = stream.ToArray();

        return bytes;
    }
}

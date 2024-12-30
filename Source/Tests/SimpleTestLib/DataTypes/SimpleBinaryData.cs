namespace SimpleTestLib.DataTypes;

public class SimpleBinaryData
{
    public SimpleBinaryData(byte[] data)
    {
        Data = data;
    }

    public byte[] Data { get; }
}

namespace SimpleTestLib.DataTypes;

public class SimpleWithPayload
{
    public SimpleWithPayload(int intValue, byte[] payload)
    {
        IntValue = intValue;
        Payload = payload;
    }

    public int IntValue { get; }
    public byte[] Payload { get; }
}

using MuniNet.Base;
using SimpleTestLib.DataTypes;

namespace SimpleTestLib;

public class CalcWithLargeBinaryData : Calculation<SimpleWithPayload, int>
{
    public const int PayloadSize = 1024 * 1024;

    public override SimpleWithPayload Calculate(int input)
    {
        var data = new byte[PayloadSize];
        for(var i=0; i<PayloadSize; i++)
        {
            data[i] = (byte)((i + input) % 256);
        }

        return new SimpleWithPayload(input, data);
    }
}


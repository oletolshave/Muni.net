using MuniNet.Base;
using SimpleTestLib.DataTypes;

namespace SimpleTestLib;

public class CalcSimpleBinaryData : Calculation<SimpleBinaryData, int>
{
    public override SimpleBinaryData Calculate(int input)
    {
        var data = new byte[input];

        for (var i=0; i<input; i++)
        {
            data[i] = (byte)(i % 256);
        }

        return new SimpleBinaryData(data);
    }
}

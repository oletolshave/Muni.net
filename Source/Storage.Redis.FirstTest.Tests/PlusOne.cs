using MuniNet.Base;

namespace FirstTestLib;

public class PlusOne : Calculation<long, long>
{
    private int _calls;

    public int CallCount => _calls;

    public override long Calculate(long input)
    {
        _calls++;

        return input + 1;
    }
}

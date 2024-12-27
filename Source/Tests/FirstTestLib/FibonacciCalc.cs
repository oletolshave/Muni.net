using MuniNet.Base;

namespace FirstTest;

public class FibonacciCalc : Calculation<int, int>
{
    private int _calls;

    public int CallCount => _calls;

    public override int Calculate(int input)
    {
        _calls++;

        return InnerCalc(input);
    }

    private int InnerCalc(int input)
    {
        if (input < 0)
            throw new ArgumentOutOfRangeException();

        if (input == 0)
            return 0;

        if (input == 1)
            return 1;

        var f0 = InnerCalc(input - 2);
        var f1 = InnerCalc(input - 1);

        return f0 + f1;
    }
}

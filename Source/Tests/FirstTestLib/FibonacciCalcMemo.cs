using MuniNet.Base;

namespace FirstTest;

public class FibonacciCalcMemo : CalculationAsync<int, int>
{
    private int _calls;
    private readonly ICacheManager _cacheManager;

    public int CallCount => _calls;

    public FibonacciCalcMemo(ICacheManager cacheManager)
    {
        _cacheManager = cacheManager;
    }

    public override ValueTask<int> CalculateAsync(int input, 
        CancellationToken cancellationToken)
    {
        _calls++;

        return InnerCalc(input);
    }

    private async ValueTask<int> InnerCalc(int input)
    {
        if (input < 0)
            throw new ArgumentOutOfRangeException();

        if (input == 0)
            return 0;

        if (input == 1)
            return 1;

        var calc = _cacheManager.For(this);
        var f0 = await calc.Calculate(input - 2);
        var f1 = await calc.Calculate(input - 1);

        return f0 + f1;
    }
}
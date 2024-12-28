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

        return InnerCalc(input, cancellationToken);
    }

    private async ValueTask<int> InnerCalc(int input,
        CancellationToken cancellationToken)
    {
        if (input < 0)
            throw new ArgumentOutOfRangeException();

        if (input == 0)
            return 0;

        if (input == 1)
            return 1;

        var calc = _cacheManager.For(this);
        var f0 = await calc.CalculateAsync(input - 2, cancellationToken);
        var f1 = await calc.CalculateAsync(input - 1, cancellationToken);

        return f0 + f1;
    }
}
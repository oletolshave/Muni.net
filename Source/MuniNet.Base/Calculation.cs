namespace MuniNet.Base;

public abstract class Calculation
{
}

public abstract class CalculationAsync<TOutput, TInput> : Calculation
{
    public abstract ValueTask<TOutput> CalculateAsync(
        TInput input, 
        CancellationToken cancellationToken = default);
}

public abstract class Calculation<TOutput, TInput> : CalculationAsync<TOutput, TInput>
{
    public abstract TOutput Calculate(
        TInput input);

    public override ValueTask<TOutput> CalculateAsync(
        TInput input, 
        CancellationToken cancellationToken = default)
    {
        var result = Calculate(input);

        return ValueTask.FromResult(result);
    }
}

public abstract class Calculation<TCalc, TOutput, TInput>
    : Calculation<TOutput, TInput>
    where TCalc : Calculation<TCalc, TOutput, TInput>
{
}

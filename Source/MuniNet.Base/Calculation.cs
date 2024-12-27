namespace MuniNet.Base;

public abstract class Calculation
{
}

public abstract class Calculation<TOutput, TInput> : Calculation
{
    public abstract TOutput Calculate(TInput input);
}

public abstract class Calculation<TCalc, TOutput, TInput>
    : Calculation<TOutput, TInput>
    where TCalc : Calculation<TCalc, TOutput, TInput>
{
}

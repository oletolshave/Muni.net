namespace MuniNet.Base;

public interface ICacheManager
{
    ICacheCalculation<TOutput, TInput>
        For<TOutput, TInput>(CalculationAsync<TOutput, TInput> calc);
}

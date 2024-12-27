namespace MuniNet.Base;

public interface ICacheManager
{
    ICacheCalculation<TOutput, TInput>
        For<TOutput, TInput>(Calculation<TOutput, TInput> calc);
}

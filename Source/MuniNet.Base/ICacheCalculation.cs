namespace MuniNet.Base;

public interface ICacheCalculation<TOutput, TInput>
{
    ValueTask<TOutput> Calculate(
        TInput input,
        CancellationToken cancellationToken = default);
}
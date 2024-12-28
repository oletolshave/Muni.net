namespace MuniNet.Base;

public interface ICacheCalculation<TOutput, TInput>
{
    ValueTask<TOutput> CalculateAsync(
        TInput input,
        CancellationToken cancellationToken = default);
}
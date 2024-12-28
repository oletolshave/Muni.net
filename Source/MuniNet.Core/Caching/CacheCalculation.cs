using MuniNet.Base;
using MuniNet.Core.Encoding;
using MuniNet.Core.Hashing;

namespace MuniNet.Core.Caching;

internal class CacheCalculation<TOutput, TInput> : ICacheCalculation<TOutput, TInput>
{
    private readonly GenericCacheManager _cacheManager;
    private readonly FunctionHash _functionHash;
    private readonly ValueEncoder<TInput> _inputValueEncoder;
    private readonly ValueEncoder<TOutput> _outputValueEncoder;
    private readonly CalculationAsync<TOutput, TInput> _calc;

    internal CacheCalculation(
        GenericCacheManager cacheManager,
        FunctionHash functionHash,
        ValueEncoder<TInput> inputValueEncoder,
        ValueEncoder<TOutput> outputValueEncoder,
        CalculationAsync<TOutput, TInput> calc)
    {
        _cacheManager = cacheManager;
        _functionHash = functionHash;
        _inputValueEncoder = inputValueEncoder;
        _outputValueEncoder = outputValueEncoder;
        _calc = calc;
    }

    public async ValueTask<TOutput> CalculateAsync(
       TInput input,
       CancellationToken cancellationToken = default)
    {
        var encodedInput = _inputValueEncoder.Encode(input);

        var cachedResult = await _cacheManager.LookupCachedValue(_functionHash, encodedInput.Span);
        if (cachedResult is not null)
        {
            var outputValue = _outputValueEncoder.Decode(cachedResult.Value.Span);

            return outputValue;
        }

        var result = await _calc.CalculateAsync(input);
        var newOutputEncoded = _outputValueEncoder.Encode(result);

        await _cacheManager.TryAddCachedValue(_functionHash, encodedInput.Span, newOutputEncoded.Span);

        return result;
    }
}

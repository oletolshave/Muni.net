using MuniNet.Base;
using MuniNet.Core.Encoding;
using MuniNet.Core.Hashing;
using MuniNet.Core.Storage;
using System.IO.Abstractions;
using System.Runtime.Loader;

namespace MuniNet.Core.Caching;

public class GenericCacheManagerClassic : ICacheManagerControl
{
    private readonly ICacheStorageEngineClassic _storageEngine;
    private readonly long _maxCacheSizeBytes;
    private readonly AssemblyHashCalculator _assemblyHashCalculator;

    public GenericCacheManagerClassic(
        IFileSystem fileSystem,
        AssemblyLoadContext assemblyLoadContext,
        ICacheStorageEngineClassic storageEngine,
        long maxCacheSizeBytes)
    {
        _assemblyHashCalculator = new AssemblyHashCalculator(fileSystem, assemblyLoadContext);
        _storageEngine = storageEngine;
        _maxCacheSizeBytes = maxCacheSizeBytes;
    }

    public ICacheCalculation<TOutput, TInput>
        For<TOutput, TInput>(CalculationAsync<TOutput, TInput> calc)
    {
        var calcType = calc.GetType();
        var functionHash = GetFunctionHash(calcType);

        var inputEncoder = new ProtobufValueEncoder<TInput>();
        var outputValueEncoder = new ProtobufValueEncoder<TOutput>();

        return new CacheCalculation<TOutput, TInput>(
            this,
            functionHash,
            inputEncoder,
            outputValueEncoder,
            calc);
    }

    public Task GarbageCollect(CancellationToken cancellationToken = default)
    {
        // TODO: Implement this

        return Task.CompletedTask;
    }

    internal ValueTask<ReadOnlyMemory<byte>?> LookupCachedValue(FunctionHash functionHash,
        ReadOnlySpan<byte> inputValue)
    {
        return _storageEngine.LookupCachedValue(functionHash, inputValue);
    }

    internal ValueTask<bool> TryAddCachedValue(FunctionHash functionHash,
        ReadOnlySpan<byte> inputValue,
        ReadOnlySpan<byte> outputValue)
    {
        return TryAddCachedValueInternal(functionHash,
            inputValue.ToArray(),
            outputValue.ToArray());
    }

    private async ValueTask<bool> TryAddCachedValueInternal(
        FunctionHash functionHash,
        ReadOnlyMemory<byte> inputValue,
        ReadOnlyMemory<byte> outputValue)
    {
        var newSize = EstimateConsumedBytes(functionHash, inputValue.Span, outputValue.Span);

        await EvictUntilEnoughRoom(newSize);

        return await _storageEngine.TryAdd(functionHash, inputValue.Span, outputValue.Span);
    }

    private async ValueTask EvictUntilEnoughRoom(long additionalBytes)
    {
        // TODO: This is not optimal at all. Please optimize.
        var currentCacheSize = await _storageEngine.ReadEstimatedCacheSize();

        if (currentCacheSize + additionalBytes > _maxCacheSizeBytes)
        {
            await _storageEngine.RemoveAllKeys();
        }
    }

    private FunctionHash GetFunctionHash(Type calcType)
    {
        var calcAssembly = calcType.Assembly;

        var asmHash = _assemblyHashCalculator.CalculateAssemblyHash(
            calcAssembly);
        string typeName = calcType.FullName ?? string.Empty;

        return FunctionHash.CalculateFunctionHash(asmHash, typeName);
    }

    internal static long EstimateConsumedBytes(FunctionHash functionHash,
        ReadOnlySpan<byte> inputValue,
        ReadOnlySpan<byte> outputValue)
    {
        return inputValue.Length + outputValue.Length;
    }
}

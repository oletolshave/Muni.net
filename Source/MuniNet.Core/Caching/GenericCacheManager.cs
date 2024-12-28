using MuniNet.Base;
using MuniNet.Core.Encoding;
using MuniNet.Core.Hashing;
using MuniNet.Core.Storage;
using System.IO.Abstractions;
using System.Runtime.Loader;

namespace MuniNet.Core.Caching;

public class GenericCacheManager
{
    private readonly ICacheStorageEngine _storageEngine;
    private readonly long _maxCacheSizeBytes;
    private readonly AssemblyHashCalculator _assemblyHashCalculator;

    public GenericCacheManager(
        IFileSystem fileSystem,
        AssemblyLoadContext assemblyLoadContext,
        ICacheStorageEngine storageEngine,
        long maxCacheSizeBytes)
    {
        _assemblyHashCalculator = new AssemblyHashCalculator(fileSystem, assemblyLoadContext);
        _storageEngine = storageEngine;
        _maxCacheSizeBytes = maxCacheSizeBytes;
    }

    public ICacheCalculation<TOutput, TInput>
        For<TOutput, TInput>(Calculation<TOutput, TInput> calc)
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
        var currentCacheSize = await _storageEngine.ReadEstimatedCacheSize();

        return await _storageEngine.TryAdd(functionHash, inputValue.Span, outputValue.Span);
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

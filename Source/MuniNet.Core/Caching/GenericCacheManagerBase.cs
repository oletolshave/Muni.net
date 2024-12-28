using MuniNet.Base;
using MuniNet.Core.Encoding;
using MuniNet.Core.Hashing;
using MuniNet.Core.Storage;
using System.IO.Abstractions;
using System.Runtime.Loader;

namespace MuniNet.Core.Caching;

public abstract class GenericCacheManagerBase
{
    private readonly AssemblyHashCalculator _assemblyHashCalculator;
    private readonly ICacheStorageEngineBase _storageEngine;

    public GenericCacheManagerBase(
        IFileSystem fileSystem,
        AssemblyLoadContext assemblyLoadContext,
        ICacheStorageEngineBase storageEngine)
    {
        _assemblyHashCalculator = new AssemblyHashCalculator(fileSystem, assemblyLoadContext);
        _storageEngine = storageEngine;
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
        await EvictUntilEnoughRoomFor(functionHash, inputValue, outputValue);

        return await _storageEngine.TryAdd(functionHash, inputValue.Span, outputValue.Span);
    }

    protected abstract Task EvictUntilEnoughRoomFor(FunctionHash functionHash,
        ReadOnlyMemory<byte> inputValue,
        ReadOnlyMemory<byte> outputValue);

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

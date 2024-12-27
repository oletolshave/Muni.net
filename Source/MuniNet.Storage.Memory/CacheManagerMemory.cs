using MuniNet.Base;
using MuniNet.Core.Encoding;
using MuniNet.Core.Hashing;
using System.IO.Abstractions;
using System.Runtime.Loader;

namespace MuniNet.Storage.Memory;

public class CacheManagerMemory
{
    private readonly MemoryStorageEngine _storageEngine;
    private readonly AssemblyHashCalculator _assemblyHashCalculator;

    public CacheManagerMemory(
        IFileSystem fileSystem,
        AssemblyLoadContext assemblyLoadContext)
    {
        _assemblyHashCalculator = new AssemblyHashCalculator(fileSystem, assemblyLoadContext);
        _storageEngine = new();
    }

    public static CacheManagerMemory New(
        IFileSystem? fileSystem = null,
        AssemblyLoadContext? assemblyLoadContext = null)
    {
        return new CacheManagerMemory(
            fileSystem ?? new FileSystem(),
            assemblyLoadContext ?? AssemblyLoadContext.Default);
    }

    public CacheManagerCalculation<TOutput, TInput>
        For<TOutput, TInput>(Calculation<TOutput, TInput> calc)
    {
        var calcType = calc.GetType();
        var functionHash = GetFunctionHash(calcType);

        var inputEncoder = new ProtobufValueEncoder<TInput>();
        var outputValueEncoder = new ProtobufValueEncoder<TOutput>();

        return new CacheManagerCalculation<TOutput, TInput>(
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
        return _storageEngine.TryAdd(functionHash, inputValue, outputValue);
    }

    private FunctionHash GetFunctionHash(Type calcType)
    {
        var calcAssembly = calcType.Assembly;

        var asmHash = _assemblyHashCalculator.CalculateAssemblyHash(
            calcAssembly);
        string typeName = calcType.FullName ?? string.Empty;

        return FunctionHash.CalculateFunctionHash(asmHash, typeName);
    }
}

public class CacheManagerCalculation<TOutput, TInput>
{
    private readonly CacheManagerMemory _cacheManager;
    private readonly FunctionHash _functionHash;
    private readonly ValueEncoder<TInput> _inputValueEncoder;
    private readonly ValueEncoder<TOutput> _outputValueEncoder;
    private readonly Calculation<TOutput, TInput> _calc;

    internal CacheManagerCalculation(
        CacheManagerMemory cacheManager,
        FunctionHash functionHash,
        ValueEncoder<TInput> inputValueEncoder,
        ValueEncoder<TOutput> outputValueEncoder,
        Calculation<TOutput, TInput> calc)
    {
        _cacheManager = cacheManager;
        _functionHash = functionHash;
        _inputValueEncoder = inputValueEncoder;
        _outputValueEncoder = outputValueEncoder;
        _calc = calc;
    }

    public async ValueTask<TOutput> Calculate(
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

        var result = _calc.Calculate(input);
        var newOutputEncoded = _outputValueEncoder.Encode(result);

        await _cacheManager.TryAddCachedValue(_functionHash, encodedInput.Span, newOutputEncoded.Span);

        return result;
    }
}

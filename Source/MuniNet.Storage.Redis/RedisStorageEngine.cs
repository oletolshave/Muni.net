using MuniNet.Core.Hashing;
using MuniNet.Core.Storage;
using StackExchange.Redis;
using System.Text;

namespace MuniNet.Storage.Redis;

internal class RedisStorageEngine : ICacheStorageEngineSelfManaged
{
    private ConnectionMultiplexer? _redis;
    private IDatabase? _database;
    private readonly string _configurationName;
    private readonly object _lock = new object();
    private readonly SemaphoreSlim _connectedSem;

    public RedisStorageEngine(string configurationName)
    {
        _configurationName = configurationName;
        _connectedSem = new SemaphoreSlim(1);
    }

    public ValueTask<ReadOnlyMemory<byte>?> LookupCachedValue(FunctionHash functionHash, ReadOnlySpan<byte> inputValue)
    {
        var redisKey = MakeRedisKey(functionHash, inputValue);

        return LookupCachedValueInner(redisKey, CancellationToken.None);
    }

    public ValueTask<bool> TryAdd(FunctionHash functionHash, ReadOnlySpan<byte> inputValue, ReadOnlySpan<byte> outputValue)
    {
        var redisKey = MakeRedisKey(functionHash, inputValue);
        var redisValue = MakeRedisValue(outputValue);

        return TryAddInner(redisKey, redisValue, CancellationToken.None);
    }

    private async ValueTask<ReadOnlyMemory<byte>?> LookupCachedValueInner(
        RedisKey redisKey, CancellationToken cancellationToken)
    {
        var db = await EnsureConnected(cancellationToken);

        var result = await db.StringGetAsync(redisKey);
        if (result.IsNull)
            return null;

        var stringResult = result.ToString();

        var converted = Convert.FromBase64String(stringResult);

        return converted;
    }

    private async ValueTask<bool> TryAddInner(
        RedisKey redisKey, 
        RedisValue redisValue,
        CancellationToken cancellationToken)
    {
        var db = await EnsureConnected(cancellationToken);

        return await db.StringSetAsync(redisKey, redisValue, TimeSpan.FromSeconds(1));
    }

    private RedisKey MakeRedisKey(FunctionHash functionHash, ReadOnlySpan<byte> value)
    {
        var sb = new StringBuilder();
        var functionHashString = Convert.ToBase64String(functionHash.HashValue.Span);

        sb.Append(functionHash);
        sb.Append('_');
        sb.Append(Convert.ToBase64String(value));

        var key = new RedisKey(sb.ToString());

        return key;
    }

    private RedisValue MakeRedisValue(ReadOnlySpan<byte> value)
    {
        var sb = new StringBuilder();
        sb.Append(Convert.ToBase64String(value));

        return new RedisValue(sb.ToString());
    }

    private async ValueTask<IDatabase> EnsureConnected(CancellationToken cancellationToken)
    {
        lock (_lock)
        {
            if (_database is not null)
                return _database;
        }

        await _connectedSem.WaitAsync(cancellationToken);
        try
        {
            lock (_lock)
            {
                if (_database is not null)
                    return _database;
            }

            // We must connect.
            _redis = await ConnectionMultiplexer.ConnectAsync(_configurationName);
            _database = _redis.GetDatabase();

            return _database;
        }

        finally
        {
            _connectedSem.Release();
        }
    }
}

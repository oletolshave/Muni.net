using MuniNet.Core.Hashing;
using MuniNet.Core.Storage;
using StackExchange.Redis;

namespace MuniNet.Storage.Redis;

internal class RedisStorageEngine : ICacheStorageEngineSelfManaged
{
    public RedisStorageEngine()
    {
        ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
        IDatabase db = redis.GetDatabase();

        db.StringSet(new RedisKey("MyTest"), new RedisValue("ok?"));

        var result = db.StringGet(new RedisKey("MyTest"));

        Math.Abs(0);
    }

    public ValueTask<ReadOnlyMemory<byte>?> LookupCachedValue(FunctionHash functionHash, ReadOnlySpan<byte> inputValue)
    {
        throw new NotImplementedException();
    }

    public ValueTask<bool> TryAdd(FunctionHash functionHash, ReadOnlySpan<byte> inputValue, ReadOnlySpan<byte> outputValue)
    {
        throw new NotImplementedException();
    }
}

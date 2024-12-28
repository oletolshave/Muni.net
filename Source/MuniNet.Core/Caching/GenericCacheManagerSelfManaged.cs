using MuniNet.Core.Hashing;
using MuniNet.Core.Storage;
using System.IO.Abstractions;
using System.Runtime.Loader;

namespace MuniNet.Core.Caching;

public class GenericCacheManagerSelfManaged : GenericCacheManagerBase
{
    public GenericCacheManagerSelfManaged(
        IFileSystem fileSystem,
        AssemblyLoadContext assemblyLoadContext,
        ICacheStorageEngineSelfManaged storageEngineSelfManaged)
        : base(fileSystem, assemblyLoadContext, storageEngineSelfManaged)
    {
      
    }

    protected override Task EvictUntilEnoughRoomFor(
        FunctionHash functionHash, ReadOnlyMemory<byte> inputValue, ReadOnlyMemory<byte> outputValue)
    {
        return Task.CompletedTask;
    }
}

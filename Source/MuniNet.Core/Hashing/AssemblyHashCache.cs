using System.Reflection;

namespace MuniNet.Core.Hashing;

public class AssemblyHashCache
{
    private readonly Dictionary<string, AssemblyHash> _cache = new();
    private object _cacheLock = new object();

    public bool TryGetValue(AssemblyName assemblyName,
        out AssemblyHash hash)
    {
        lock (_cacheLock)
        {
            return _cache.TryGetValue(assemblyName.FullName, out hash);
        }
    }

    public bool TryAdd(AssemblyName assemblyName, AssemblyHash hash)
    {
        lock (_cacheLock)
        {
            return _cache.TryAdd(assemblyName.FullName, hash);
        }
    }
}

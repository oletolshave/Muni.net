using System.IO.Abstractions;
using System.Reflection;
using System.Runtime.Loader;

namespace MuniNet.Core.Hashing;

public class AssemblyHashCalculator
{
    private readonly IFileSystem _fileSystem;
    private readonly AssemblyLoadContext _assemblyLoadContext;
    private readonly AssemblyHashCache _hashCache = new();

    public AssemblyHashCalculator(
        IFileSystem fileSystem,
        AssemblyLoadContext assemblyLoadContext)
    {
        _fileSystem = fileSystem;
        _assemblyLoadContext = assemblyLoadContext;
    }

    public AssemblyHash CalculateAssemblyHash(
        Assembly assembly)
    {
        var assemblyName = assembly.GetName();
        if (_hashCache.TryGetValue(assemblyName, out var hash))
            return hash;

        var visited = new HashSet<string>();

        return CalculateAssemblyHash(
            visited,
            assemblyName);
    }

    private AssemblyHash CalculateAssemblyHash(
        HashSet<string> visited,
        AssemblyName assemblyName)
    {
        // TODO: Instead of resolving the assembly by name, and then reloading the assembly to get the bytes,
        // it should be possible to re-use the loaded bytes for both hash calculation and the loaded assembly.

        var assembly = _assemblyLoadContext.LoadFromAssemblyName(assemblyName);

        var hashCalcContext = new HashCalculationContext();

        var referencedAssemblies = assembly.GetReferencedAssemblies();
        foreach (var refAsm in referencedAssemblies.OrderBy(r => r.FullName))
        {
            if (!_hashCache.TryGetValue(refAsm, out var refAsmHash))
            {
                string refAsmName = refAsm.FullName;

                if (visited.Contains(refAsmName))
                {
                    throw new InvalidOperationException("Infinite recursion.");
                }

                visited.Add(refAsmName);

                refAsmHash = CalculateAssemblyHash(visited, refAsm);
                visited.Remove(refAsmName);
            }

            hashCalcContext.FeedBytes(refAsmHash.HashValue.Span);
        }

        string? path = assembly.Location;
        if (!string.IsNullOrEmpty(path)
            && _fileSystem.File.Exists(path))
        {
            var assemblyBytes = _fileSystem.File.ReadAllBytes(path);
            hashCalcContext.FeedBytes(assemblyBytes);
        }

        // Actually calculate the hash from all the bytes.
        var hashResult = hashCalcContext.GetFinalHashBytes();
        if (hashResult is null)
            throw new InvalidOperationException("No hash could be generated.");

        var hash = new AssemblyHash(hashResult.Value.algorithm,
            hashResult.Value.hashValue);

        _hashCache.TryAdd(assemblyName, hash);

        return hash;
    }

}

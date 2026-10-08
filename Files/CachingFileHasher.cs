using System.Collections.Concurrent;
using System.Security.Cryptography;

namespace DuplicateFinder.Files;

internal abstract class CachingFileHasher : FileHasher, ICachingFileHasher
{
    private readonly ConcurrentDictionary<string, string> _cache = new();

    protected CachingFileHasher(HashAlgorithm hashAlgorithm)
        : base(hashAlgorithm)
    {
    }

    public override string ComputeFileHash(string fileFullName)
    {
        return _cache.GetOrAdd(fileFullName, base.ComputeFileHash);
    }

    public void ClearCache()
    {
        _cache.Clear();
    }

    public bool ConfirmCache(string[] expectedKeys)
    {
        return expectedKeys.All(x => _cache.ContainsKey(x));
    }
}
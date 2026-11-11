using System.Security.Cryptography;

using DuplicateFinder.Caches;

namespace DuplicateFinder.Files.FileHashers;

internal abstract class CachingFileHasher(HashAlgorithm hashAlgorithm) : FileHasher(hashAlgorithm)
{
    private readonly FileHashCache _cache = new();

    public override string ComputeFileHash(string fileFullName)
    {
        return _cache.GetOrCreateAsync(
            fileFullName,
            x => Task.Run(() => base.ComputeFileHash(x)),
            CancellationToken.None)
            .GetAwaiter()
            .GetResult();
    }

    private class FileHashCache()
        : Cache<string, string>(new SemaphoreSlimCache<string>())
    {
    }

    public override void Dispose()
    {
        _cache.Dispose();
        base.Dispose();
    }
}
using System.Security.Cryptography;
using DuplicateFinder.Caches;
using DuplicateFinder.Files.FileHashers;

namespace DuplicateFinder.Files;

internal abstract class CachingFileHasher(HashAlgorithm hashAlgorithm) : FileHasher(hashAlgorithm)
{
    private readonly FileHashCache _cache = new();

    public override string ComputeFileHash(string fileFullName)
    {
        return _cache.GetOrCreateAsync(
            fileFullName,
            x => Task.Run(() => base.ComputeFileHash(x)),
            CancellationToken.None).GetAwaiter().GetResult();
    }

    private class FileHashCache()
        : Cache<string, string>(Semaphore)
    {
        private static readonly SemaphoreSlim Semaphore = new(1, 1);
    }
}
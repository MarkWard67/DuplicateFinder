using System.Security.Cryptography;

namespace DuplicateFinder.Files;

internal class CachingSHA256FileHasher : CachingFileHasher
{
    public CachingSHA256FileHasher()
        : base(SHA256.Create())
    {
    }
}
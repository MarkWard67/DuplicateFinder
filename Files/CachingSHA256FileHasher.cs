using System.Security.Cryptography;

namespace DuplicateFinder.Files;

internal class CachingSha256FileHasher() : CachingFileHasher(SHA256.Create());
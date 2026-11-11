using System.Security.Cryptography;

namespace DuplicateFinder.Files.FileHashers;

internal class CachingSha256FileHasher() : CachingFileHasher(SHA256.Create());
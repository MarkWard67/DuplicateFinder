using System.Security.Cryptography;
using System.Text;

namespace DuplicateFinder.Files;

internal abstract class FileHasher(HashAlgorithm hashAlgorithm) : IFileHasher
{
    public virtual string ComputeFileHash(string fileFullName)
    {
        var lastAccessTime = File.GetLastAccessTimeUtc(fileFullName);
        try
        {
            using var stream = File.OpenRead(fileFullName);

            // Convert the input string to a byte array and compute the hash.
            var data = hashAlgorithm.ComputeHash(stream);

            // Create a new StringBuilder to collect the bytes
            // and create a string.
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            foreach (var t in data) sBuilder.Append(t.ToString("x2"));

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
        finally
        {
            File.SetLastAccessTimeUtc(fileFullName, lastAccessTime);
        }
    }
}
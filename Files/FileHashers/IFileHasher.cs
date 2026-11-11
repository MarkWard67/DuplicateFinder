namespace DuplicateFinder.Files.FileHashers;

internal interface IFileHasher : IDisposable
{
    public string ComputeFileHash(string fileFullName);

}

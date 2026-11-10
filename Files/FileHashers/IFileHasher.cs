namespace DuplicateFinder.Files.FileHashers;

internal interface IFileHasher
{
    public string ComputeFileHash(string fileFullName);
}

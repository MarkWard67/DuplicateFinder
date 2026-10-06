namespace DuplicateFinder.Files;

internal interface IFileHasher
{
    public string ComputeFileHash(string fileFullName);
}
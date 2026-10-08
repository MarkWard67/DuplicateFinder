namespace DuplicateFinder.Files;

internal interface IFileHasher
{
    public string ComputeFileHash(string fileFullName);
}

internal interface ICachingFileHasher : IFileHasher
{
    public void ClearCache();

    public bool ConfirmCache(string[] expectedKeys);
}
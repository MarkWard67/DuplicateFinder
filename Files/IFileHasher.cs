namespace DuplicateFinder.Files;

internal interface IFileHasher
{
    public string ComputeFileHash(string fileFullName);
    void Clear();
    void ClearIf(int maximumItems);
}

internal interface ICachingFileHasher : IFileHasher
{
    public void ClearCache();

    public bool ConfirmCache(string[] expectedKeys);
}
namespace DuplicateFinder.Files.FileExcluders;

internal class ExplicitFileExcluder(string[] excludedFileFullNames) : IFileExcluder
{
    private readonly HashSet<string> _excludedFileFullNames = excludedFileFullNames.Distinct().ToHashSet();

    public bool ExcludeFile(FileInfo fileInfo)
    {
        return _excludedFileFullNames.Contains(fileInfo.FullName);
    }
}
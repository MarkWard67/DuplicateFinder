namespace DuplicateFinder.Files;

internal class ExplicitFileExcluder : IFileExcluder
{
    private readonly HashSet<string>? _excludedFileFullNames;

    public ExplicitFileExcluder()
    {
    }

    public ExplicitFileExcluder(string[] excludedFileFullNames)
    {
        _excludedFileFullNames = excludedFileFullNames.Distinct().ToHashSet();
    }

    public bool ExcludeFile(FileInfo fileInfo)
    {
        return _excludedFileFullNames?.Contains(fileInfo.FullName) == true;
    }
}
namespace DuplicateFinder.Folders;

internal class ExplicitFolderExcluder : IFolderExcluder
{
    private readonly HashSet<string>? _excludedFolders;

    private ExplicitFolderExcluder()
    {
    }

    public ExplicitFolderExcluder(string[] excludedFolders)
    {
        _excludedFolders = excludedFolders.Distinct().ToHashSet();
    }

    public static ExplicitFolderExcluder DefaultInstance { get; } = new();

    public bool ExcludeFolder(string folderFullPath)
    {
        return _excludedFolders?.Contains(folderFullPath) == true;
    }
}
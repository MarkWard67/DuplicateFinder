namespace DuplicateFinder.Folders;

internal class DoNotExcludeExplicitFolderExcluder : IFolderExcluder
{
    private readonly HashSet<string> _excludedFolders;

    public DoNotExcludeExplicitFolderExcluder(string[] excludedFolders)
    {
        _excludedFolders = excludedFolders.Distinct().ToHashSet();
    }

    public bool ExcludeFolder(string folderFullPath)
    {
        return _excludedFolders.Contains(folderFullPath);
    }
}
namespace DuplicateFinder.Folders;

internal class ExplicitFolderExcluder(string[] excludedFolders) : IFolderExcluder
{
    private readonly HashSet<string>? _excludedFolders = excludedFolders.Distinct().ToHashSet();

    public bool ExcludeFolder(string folderFullPath)
    {
        return _excludedFolders?.Contains(folderFullPath) == true;
    }
}
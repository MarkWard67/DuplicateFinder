using DuplicateFinder.Files;

namespace DuplicateFinder.Folders;

internal class AggregateFolderExcluder(params IEnumerable<IFolderExcluder> excluders) : IFolderExcluder
{
    private readonly IFolderExcluder[] _excluders = excluders.ToArray();

    public bool ExcludeFolder(string folderFullPath)
    {
        return _excluders.Any(x => x.ExcludeFolder(folderFullPath));
    }
}
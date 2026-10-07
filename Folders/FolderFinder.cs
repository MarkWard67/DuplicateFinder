namespace DuplicateFinder.Folders;

internal class FolderFinder(
    string[] rootFolderPaths,
    string folderSpecification,
    bool recursive,
    IFolderExcluder? folderExcluder = null)
    : IFolderFinder
{
    private readonly string[] _rootFolderPaths = rootFolderPaths.Distinct().ToArray();

    public FolderFinder(
        string rootFolderPath,
        string folderSpecification,
        bool recursive,
        IFolderExcluder? folderExcluder = null)
    : this(
        [rootFolderPath],
        folderSpecification,
        recursive,
        folderExcluder)
    {
    }

    public DirectoryInfo[] FindFolders()
    {
        return _rootFolderPaths
            .SelectMany(x =>
            {
                var ret = new List<string> { x };

                ret.AddRange(
                    Directory.GetDirectories(
                        x,
                        folderSpecification,
                        new EnumerationOptions
                        {
                            RecurseSubdirectories = recursive,
                            MatchType = MatchType.Win32,
                            IgnoreInaccessible = true,
                            AttributesToSkip = FileAttributes.Hidden | FileAttributes.System,
                            MaxRecursionDepth = int.MaxValue
                        }));

                return ret;
            })
            .Distinct()
            .Where(x => folderExcluder == null || !folderExcluder.ExcludeFolder(x))
            .Select(x => new DirectoryInfo(x))
            .Where(x => x.Exists)
            .ToArray();
    }
}
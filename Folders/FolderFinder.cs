namespace DuplicateFinder.Folders;

internal class FolderFinder : IFolderFinder
{
    private readonly IFolderExcluder _folderExcluder;
    private readonly string _folderSpecification;
    private readonly bool _recursive;
    private readonly string[] _rootFolderPaths;

    public FolderFinder(
        string rootPath,
        string folderSpecification,
        bool recursive,
        IFolderExcluder folderExcluder,
        params string[] additionalRootFolderPaths)
    {
        _folderSpecification = folderSpecification;
        _recursive = recursive;
        _folderExcluder = folderExcluder;
        var rootFolderPaths = new List<string> { rootPath };
        rootFolderPaths.AddRange(additionalRootFolderPaths);
        _rootFolderPaths = rootFolderPaths.Distinct().ToArray();
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
                        _folderSpecification,
                        new EnumerationOptions
                        {
                            RecurseSubdirectories = _recursive,
                            MatchType = MatchType.Win32,
                            IgnoreInaccessible = true,
                            AttributesToSkip = FileAttributes.Hidden | FileAttributes.System,
                            MaxRecursionDepth = int.MaxValue
                        }));

                return ret;
            })
            .Distinct()
            .Where(x => !_folderExcluder.ExcludeFolder(x))
            .Select(x => new DirectoryInfo(x))
            .Where(x => x.Exists)
            .ToArray();
    }
}
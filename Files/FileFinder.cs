using DuplicateFinder.Folders;

namespace DuplicateFinder.Files;

internal class FileFinder : IFileFinder
{
    private readonly IFileExcluder _fileExcluder;
    private readonly IFolderFinder _folderFinder;

    public FileFinder(
        IFolderFinder folderFinder,
        IFileExcluder fileExcluder)
    {
        _folderFinder = folderFinder;
        _fileExcluder = fileExcluder;
    }

    public FileInfo[] FindFiles(
        bool recursive = false,
        string fileSpecification = "*")
    {
        var folders = _folderFinder.FindFolders();

        return folders
            .SelectMany(dir => dir.GetFiles(fileSpecification, SearchOption.TopDirectoryOnly))
            .Where(f => !f.Attributes.HasFlag(FileAttributes.Directory) && !_fileExcluder.ExcludeFile(f))
            .ToArray();
    }
}
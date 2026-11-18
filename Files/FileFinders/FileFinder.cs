using DuplicateFinder.Files.FileExcluders;
using DuplicateFinder.Folders.FolderFinders;

namespace DuplicateFinder.Files.FileFinders;

internal class FileFinder(
    IFolderFinder folderFinder,
    bool recursive = false,
    string fileSpecification = "*",
    IFileExcluder? fileExcluder = null)
    : IFileFinder
{
    public FileInfo[] FindFiles()
    {
        var folders = folderFinder.FindFolders();
        var searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

        return folders
            .SelectMany(dir => dir.GetFiles(fileSpecification, searchOption))
            .Where(f => !f.Attributes.HasFlag(FileAttributes.Directory)
                        && (fileExcluder == null || !fileExcluder.ExcludeFile(f)))
            .OrderBy(x => x.FullName)
            .ToArray();
    }
}
using DuplicateFinder.Folders;

namespace DuplicateFinder.Files;

internal class FileFinder(
    IFolderFinder folderFinder,
    IFileExcluder? fileExcluder = null)
    : IFileFinder
{
    public FileInfoWrapper[] FindFiles(
        bool recursive = false,
        string fileSpecification = "*")
    {
        var folders = folderFinder.FindFolders();

        return folders
            .SelectMany(dir => dir.GetFiles(fileSpecification, SearchOption.TopDirectoryOnly))
            .Where(f => !f.Attributes.HasFlag(FileAttributes.Directory) && (
                fileExcluder == null || !fileExcluder.ExcludeFile(f)))
            .Select(f => new FileInfoWrapper(f))
            .ToArray();
    }
}
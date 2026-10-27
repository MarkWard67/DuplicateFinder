namespace DuplicateFinder.Files.FileExcluders;

internal interface IFileExcluder
{
    bool ExcludeFile(FileInfo fileInfo);
}
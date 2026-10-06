namespace DuplicateFinder.Files;

internal interface IFileExcluder
{
    bool ExcludeFile(FileInfo fileInfo);
}
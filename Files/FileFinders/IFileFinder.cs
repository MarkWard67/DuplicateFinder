namespace DuplicateFinder.Files.FileFinders;

internal interface IFileFinder
{
    FileInfo[] FindFiles();
}
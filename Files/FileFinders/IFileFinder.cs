namespace DuplicateFinder.Files.FileFinders;

internal interface IFileFinder
{
    FileInfo[] FindFiles(
        bool recursive = false,
        string fileSpecification = "*");
}
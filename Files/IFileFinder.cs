namespace DuplicateFinder.Files;

internal interface IFileFinder
{
    FileInfo[] FindFiles(
        bool recursive = false,
        string fileSpecification = "*");
}
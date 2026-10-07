namespace DuplicateFinder.Files;

internal interface IFileFinder
{
    FileInfoWrapper[] FindFiles(
        bool recursive = false,
        string fileSpecification = "*");
}
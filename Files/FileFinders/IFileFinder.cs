namespace DuplicateFinder.Files.FileFinders;

internal interface IFileFinder
{
    FileInfoWrapper[] FindFiles(
        bool recursive = false,
        string fileSpecification = "*");
}
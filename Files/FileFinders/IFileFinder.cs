namespace DuplicateFinder.Files.FileFinders;

internal interface IFileFinder : IDisposable
{
    FileInfoWrapper[] FindFiles(
        bool recursive = false,
        string fileSpecification = "*");
}
namespace DuplicateFinder.Files.FileFinders;

internal interface IFileFinder : IDisposable
{
    FileInfo[] FindFiles(
        bool recursive = false,
        string fileSpecification = "*");
}
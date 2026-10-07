namespace DuplicateFinder.Files;

internal class FileExtensionExcluder(string[] fileExtensions)
    : IFileExcluder
{
    private readonly HashSet<string> _fileExtensions =
        fileExtensions.Distinct().ToHashSet(StringComparer.InvariantCultureIgnoreCase);

    public bool ExcludeFile(FileInfo fileInfo)
    {
        return _fileExtensions.Contains(fileInfo.Extension);
    }
}
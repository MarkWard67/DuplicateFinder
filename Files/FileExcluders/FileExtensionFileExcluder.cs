namespace DuplicateFinder.Files.FileExcluders;

internal class FileExtensionFileExcluder(params IEnumerable<string> fileExtensions)
    : IFileExcluder
{
    private readonly HashSet<string> _fileExtensions = fileExtensions
        .Distinct(StringComparer.InvariantCultureIgnoreCase)
        .ToHashSet(StringComparer.InvariantCultureIgnoreCase);

    public bool ExcludeFile(FileInfo fileInfo)
    {
        return _fileExtensions.Contains(fileInfo.Extension);
    }
}
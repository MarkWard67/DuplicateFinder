namespace DuplicateFinder.Files.FileExcluders;

internal class FileNameFileExcluder(params IEnumerable<string> fileNames)
    : IFileExcluder
{
    private readonly HashSet<string> _fileNames = fileNames
            .Distinct(StringComparer.InvariantCultureIgnoreCase)
            .ToHashSet(StringComparer.InvariantCultureIgnoreCase);

    public bool ExcludeFile(FileInfo fileInfo)
    {
        return _fileNames.Contains(fileInfo.Name);
    }
}
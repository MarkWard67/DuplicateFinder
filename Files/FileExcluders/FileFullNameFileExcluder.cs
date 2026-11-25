namespace DuplicateFinder.Files.FileExcluders;

internal class FileFullNameFileExcluder(params IEnumerable<string> fileFullNames)
    : IFileExcluder
{
    private readonly HashSet<string> _fileNames = fileFullNames
            .Distinct(StringComparer.InvariantCultureIgnoreCase)
            .ToHashSet(StringComparer.InvariantCultureIgnoreCase);

    public bool ExcludeFile(FileInfo fileInfo)
    {
        return _fileNames.Contains(fileInfo.FullName);
    }
}
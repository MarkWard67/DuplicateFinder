namespace DuplicateFinder.Files;

internal class FileExtensionExcluder(
    string[] fileExtensions,
    IFileExcluder? innerExcluder = null)
    : IFileExcluder
{
    private readonly HashSet<string> _fileExtensions =
        fileExtensions.Distinct().ToHashSet(StringComparer.InvariantCultureIgnoreCase);

    public bool ExcludeFile(FileInfo fileInfo)
    {
        if (_fileExtensions.Contains(fileInfo.Extension))
        {
            return true;
        }

        return innerExcluder?.ExcludeFile(fileInfo) == true;
    }
}
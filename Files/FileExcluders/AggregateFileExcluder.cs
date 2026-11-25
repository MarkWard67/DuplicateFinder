namespace DuplicateFinder.Files.FileExcluders;

internal class AggregateFileExcluder(params IEnumerable<IFileExcluder> fileExcluders)
    : IFileExcluder
{
    private readonly IFileExcluder[] _fileExcluders = fileExcluders.ToArray();

    public bool ExcludeFile(FileInfo fileInfo)
    {
        return _fileExcluders.Any(x => x.ExcludeFile(fileInfo));
    }
}
namespace DuplicateFinder.Files.FileExcluders;

internal class AggregateFileExcluder(params IEnumerable<IFileExcluder> excluders) : IFileExcluder
{
    private readonly IFileExcluder[] _excluders = [.. excluders];

    public static IFileExcluder ExcludeNone => new AggregateFileExcluder();

    public bool ExcludeFile(FileInfo fileInfo)
    {
        return _excluders.Any(x => x.ExcludeFile(fileInfo));
    }
}
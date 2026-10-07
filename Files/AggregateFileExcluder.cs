namespace DuplicateFinder.Files;

internal class AggregateFileExcluder(params IEnumerable<IFileExcluder> excluders) : IFileExcluder
{
    private readonly IFileExcluder[] _excluders = excluders.ToArray();

    public bool ExcludeFile(FileInfo fileInfo)
    {
        return _excluders.Any(x => x.ExcludeFile(fileInfo));
    }
}
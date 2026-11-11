namespace DuplicateFinder.Files.FileGroupings;

internal class IdenticalFileInfoGroupingFactory : IFileInfoGroupingFactory
{
    public static IdenticalFileInfoGroupingFactory Instance { get; } = new();

    private IdenticalFileInfoGroupingFactory()
    {
    }

    public IFileInfoGrouping CreateFileInfoGrouping(
        IEnumerable<FileInfo> fileInfos,
        FileInfoGroupingKey? key = null)
    {
        return new IdenticalFileInfoGrouping(fileInfos, key ?? new FileInfoGroupingKey());
    }
}
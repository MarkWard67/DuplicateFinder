namespace DuplicateFinder.Files.FileGroupings;

internal class IdenticalFileInfoGroupingFactory : IFileInfoGroupingFactory
{
    public static IdenticalFileInfoGroupingFactory Instance { get; } = new IdenticalFileInfoGroupingFactory();

    private IdenticalFileInfoGroupingFactory()
    {
    }

    public IFileInfoGrouping CreateFileInfoGrouping(
        IEnumerable<FileInfoWrapper> fileInfos,
        FileInfoGroupingKey? key = null)
    {
        return new IdenticalFileInfoGrouping(fileInfos, key ?? new FileInfoGroupingKey());
    }
}
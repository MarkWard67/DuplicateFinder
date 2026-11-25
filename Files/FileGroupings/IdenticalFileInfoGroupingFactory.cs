namespace DuplicateFinder.Files.FileGroupings;

internal class IdenticalFileInfoGroupingFactory : IFileInfoGroupingFactory
{
    private IdenticalFileInfoGroupingFactory()
    {
    }

    public static IdenticalFileInfoGroupingFactory Instance { get; } = new();

    public IFileInfoGrouping CreateFileInfoGrouping(
        IEnumerable<FileInfo> fileInfos,
        FileInfoGroupingKey? key = null)
    {
        return new IdenticalFileInfoGrouping(key ?? new FileInfoGroupingKey(), fileInfos);
    }

    public IFileInfoGrouping CreateFileInfoGrouping(
        IEnumerable<IFileInfoTuple> fileInfoTuples,
        FileInfoGroupingKey? key = null)
    {
        return new IdenticalFileInfoGrouping(key ?? new FileInfoGroupingKey(), fileInfoTuples);
    }

    public IFileInfoGrouping CreateFileInfoGrouping(IFileInfoGrouping fileInfoGrouping)
    {
        if (fileInfoGrouping.Count() > 1 && fileInfoGrouping.All(x => x is FileInfo1Tuple))
        {
            return new IdenticalFileInfoGrouping(
                fileInfoGrouping.Key ?? new FileInfoGroupingKey(),
                new FileInfoNTuple(fileInfoGrouping.Cast<FileInfo1Tuple>().Select(t => t.FileInfo1)));
        }

        return new IdenticalFileInfoGrouping(
            fileInfoGrouping.Key ?? new FileInfoGroupingKey(),
            fileInfoGrouping);
    }
}
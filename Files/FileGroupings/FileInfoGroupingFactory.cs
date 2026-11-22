namespace DuplicateFinder.Files.FileGroupings;

internal class FileInfoGroupingFactory : IFileInfoGroupingFactory
{
    public static FileInfoGroupingFactory Instance { get; } = new();

    private FileInfoGroupingFactory()
    {
    }

    public IFileInfoGrouping CreateFileInfoGrouping(
        IEnumerable<FileInfo> fileInfos,
        FileInfoGroupingKey? key = null)
    {
        return new FileInfoGrouping(key, fileInfos);
    }

    public IFileInfoGrouping CreateFileInfoGrouping(
        IEnumerable<IFileInfoTuple> fileInfoTuples,
        FileInfoGroupingKey? key = null)
    {
        return new FileInfoGrouping(key, fileInfoTuples);
    }
}
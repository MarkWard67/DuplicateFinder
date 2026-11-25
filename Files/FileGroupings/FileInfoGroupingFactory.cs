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
        return new FileInfoGrouping(
            FileInfoGroupingType.None,
            key,
            fileInfos);
    }

    public IFileInfoGrouping CreateFileInfoGrouping(
        IEnumerable<IFileInfoTuple> fileInfoTuples,
        FileInfoGroupingKey? key = null)
    {
        return new FileInfoGrouping(
            FileInfoGroupingType.None,
            key,
            fileInfoTuples);
    }

    public IFileInfoGrouping CreateFileInfoGrouping(IFileInfoGrouping fileInfoGrouping)
    {
        return fileInfoGrouping;
    }
}
using DuplicateFinder.Files.FileGroupings;

namespace DuplicateFinder.Files.FileGroupers;

internal class NoopFileGrouper : IFileGrouper
{
    private NoopFileGrouper()
    {
    }

    public static NoopFileGrouper Instance = new();

    public IEnumerable<IFileInfoGrouping> GroupFiles(
        IEnumerable<FileInfo> fileInfos,
        FileInfoGroupingKey? key = null)
    {
        return GroupFiles(
            fileInfos,
            FileInfoGroupingFactory.Instance,
            key);
    }

    public IEnumerable<IFileInfoGrouping> GroupFiles(
        IEnumerable<FileInfo> fileInfos,
        IFileInfoGroupingFactory groupingFactory,
        FileInfoGroupingKey? key = null)
    {
        return [groupingFactory.CreateFileInfoGrouping(fileInfos, key)];
    }

    public void Dispose()
    {
        // TODO release managed resources here
    }
}
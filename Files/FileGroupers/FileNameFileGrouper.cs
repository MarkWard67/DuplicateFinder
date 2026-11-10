using DuplicateFinder.Files.FileGroupings;

namespace DuplicateFinder.Files.FileGroupers;

internal class FileNameFileGrouper : IFileGrouper
{
    private FileNameFileGrouper()
    {
    }

    public static FileNameFileGrouper Instance = new();

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
        var result = fileInfos
            .GroupBy(f => f.Name)
            .Select(g => groupingFactory.CreateFileInfoGrouping(
                g,
                FileInfoGroupingKey.Extend(key, g.Key)))
            .ToArray();
        return result;
    }
}
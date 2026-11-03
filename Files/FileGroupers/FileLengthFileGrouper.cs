using DuplicateFinder.Files.FileGroupings;

namespace DuplicateFinder.Files.FileGroupers;

internal class FileLengthFileGrouper : IFileGrouper
{
    private FileLengthFileGrouper()
    {
    }

    public static FileLengthFileGrouper Instance = new();

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
            .GroupBy(f => f.Length)
            .Select(g => groupingFactory.CreateFileInfoGrouping(
                g,
                FileInfoGroupingKey.Extend(key, g.Key)))
            .ToArray();
        return result;
    }
}
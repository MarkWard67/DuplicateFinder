using DuplicateFinder.Files.FileGroupings;

namespace DuplicateFinder.Files.FileGroupers;

internal class FileLengthFileGrouper : IFileGrouper
{
    private FileLengthFileGrouper()
    {
    }

    public static FileLengthFileGrouper Instance = new();

    public IEnumerable<IFileInfoGrouping> GroupFiles(
        IEnumerable<FileInfoWrapper> fileInfos,
        FileInfoGroupingKey? key = null)
    {
        return GroupFiles(
            fileInfos,
            FileInfoGroupingFactory.Instance,
            key);
    }

    public IEnumerable<IFileInfoGrouping> GroupFiles(
        IEnumerable<FileInfoWrapper> fileInfos,
        IFileInfoGroupingFactory groupingFactory,
        FileInfoGroupingKey? key = null)
    {
        var result = fileInfos
            .GroupBy(f => f.FileInfo.Length)
            .Select(g => groupingFactory.CreateFileInfoGrouping(
                g,
                FileInfoGroupingKey.Extend(key, g.Key)))
            .ToArray();
        return result;
    }
}
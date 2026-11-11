using DuplicateFinder.Files.FileGroupings;

namespace DuplicateFinder.Files.FileGroupers;

internal class DirectoryNameFileGrouper : IFileGrouper
{
    public IEnumerable<IFileInfoGrouping> GroupFiles(
        IEnumerable<FileInfo> fileInfos,
        FileInfoGroupingKey? key = null)
    {
        return GroupFiles(
            fileInfos,
            FileInfoGroupingFactory.Instance,
            key);
    }

    public IEnumerable<IFileInfoGrouping> GroupFiles(IEnumerable<FileInfo> fileInfos, IFileInfoGroupingFactory groupingFactory,
        FileInfoGroupingKey? key = null)
    {
        var result = fileInfos
            .GroupBy(f => f.DirectoryName)
            .Select(g => new FileInfoGrouping(
                g,
                FileInfoGroupingKey.Extend(key, g.Key)))
            .ToArray();
        return result;
    }

    public void Dispose()
    {
        // TODO release managed resources here
    }
}
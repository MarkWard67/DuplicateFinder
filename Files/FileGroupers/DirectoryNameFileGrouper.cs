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
            .Select(g => new FileInfoGrouping(FileInfoGroupingKey.Extend(key, g.Key), g))
            .ToArray();
        return result;
    }

    public void Dispose()
    {
        // TODO release managed resources here
    }
}
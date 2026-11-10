using DuplicateFinder.Files.FileGroupings;
using DuplicateFinder.Files.FileHashers;

namespace DuplicateFinder.Files.FileGroupers;

internal class FileHashFileGrouper(IFileHasher fileHasher) : IFileGrouper
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
        var fileInfoArray = fileInfos.ToArray();

        if (fileInfoArray.Length < 2)
        {
            return [new FileInfoGrouping(fileInfoArray, key)];
        }

        var result = fileInfoArray
            .GroupBy(f => fileHasher.ComputeFileHash(f.FullName))
            .Select(g => groupingFactory.CreateFileInfoGrouping(
                g,
                FileInfoGroupingKey.Extend(key, g.Key)))
            .ToArray();
        return result;
    }
}
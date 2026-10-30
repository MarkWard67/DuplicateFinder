using DuplicateFinder.Files.FileGroupings;

namespace DuplicateFinder.Files.FileGroupers;

internal class AggregateFileGrouper(
    IFileInfoGroupingFactory groupingFactory,
    params IFileGrouper[] fileGroupers) : IFileGrouper
{
    public IEnumerable<IFileInfoGrouping> GroupFiles(
        IEnumerable<FileInfoWrapper> fileInfos,
        FileInfoGroupingKey? key = null)
    {
        return GroupFiles(
            fileInfos,
            groupingFactory,
            key);
    }

    public IEnumerable<IFileInfoGrouping> GroupFiles(
        IEnumerable<FileInfoWrapper> fileInfos,
        IFileInfoGroupingFactory groupingFactoryOverride,
        FileInfoGroupingKey? key = null)
    {
        var result = new List<IFileInfoGrouping> { new FileInfoGrouping(fileInfos, key) };

        foreach (var fileGrouper in fileGroupers)
        {
            var ret = new List<IFileInfoGrouping>();
            foreach (var grouping in result)
            {
                ret.AddRange(
                    fileGrouper.GroupFiles(
                        grouping,
                        groupingFactoryOverride,
                        grouping.Key));
            }

            result = ret;
        }

        return result.ToArray();
    }
}
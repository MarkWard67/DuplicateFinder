using DuplicateFinder.Files.FileGroupings;

namespace DuplicateFinder.Files.FileGroupers;

internal class AggregateFileGrouper(params IFileGrouper[] fileGroupers) : IFileGrouper
{
    public IEnumerable<IFileInfoGrouping> GroupFiles(IEnumerable<FileInfoWrapper> fileInfos)
    {
        var result = new List<IFileInfoGrouping> { new FileInfoGrouping(fileInfos) };

        foreach (var fileGrouper in fileGroupers)
        {
            var ret = new List<IFileInfoGrouping>();
            foreach (var grouping in result) ret.AddRange(fileGrouper.GroupFiles(grouping));
            result = ret;
        }

        return result.ToArray();
    }
}
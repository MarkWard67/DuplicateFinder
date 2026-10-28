using DuplicateFinder.Files.FileGroupings;

namespace DuplicateFinder.Files.FileGroupers;

internal class AcrossFoldersFileGrouper : IFileGrouper
{
    public IEnumerable<IFileInfoGrouping> GroupFiles(IEnumerable<FileInfoWrapper> fileInfos)
    {
        return fileInfos
            .GroupBy(f => f.FileInfo.Length)
            .Select(g => new FileInfoGrouping(g));
    }
}
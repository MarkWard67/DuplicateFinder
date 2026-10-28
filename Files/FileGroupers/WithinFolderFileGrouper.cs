using DuplicateFinder.Files.FileGroupings;

namespace DuplicateFinder.Files.FileGroupers;

internal class WithinFolderFileGrouper : IFileGrouper
{
    public IEnumerable<IFileInfoGrouping> GroupFiles(IEnumerable<FileInfoWrapper> fileInfos)
    {
        return fileInfos
            .GroupBy(f => (f.FileInfo.DirectoryName, f.FileInfo.Length))
            .Select(g => new FileInfoGrouping(g));
    }
}
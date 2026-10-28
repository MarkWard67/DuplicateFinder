using DuplicateFinder.Files.FileGroupings;

namespace DuplicateFinder.Files.FileGroupers;

internal class DeduplicatingFileGrouper(IFileHasher fileHasher) : IFileGrouper
{
    public IEnumerable<IFileInfoGrouping> GroupFiles(IEnumerable<FileInfoWrapper> fileInfos)
    {
        var fileInfoArray = fileInfos.ToArray();

        if (fileInfoArray.Length < 2) return [new FileInfoGrouping(fileInfoArray)];

        return fileInfoArray
            .GroupBy(f => (f.FileInfo.Length, FileHash: fileHasher.ComputeFileHash(f.FileInfo.FullName)))
            .Select(g => new IdenticalFileInfoGrouping(
                g.Key.Length,
                g.Key.FileHash,
                g));
    }
}
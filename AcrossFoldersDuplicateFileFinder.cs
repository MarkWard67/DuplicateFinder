using DuplicateFinder.Files;

namespace DuplicateFinder;

internal class AcrossFoldersDuplicateFileFinder(
    IFileFinder fileFinder,
    IFileHasher fileHasher)
{
    public FileInfoWrapper[][] FindDuplications()
    {
        var allFileInfos = fileFinder.FindFiles();

        var possibleDuplicates = allFileInfos
            .GroupBy(f => f.FileInfo.Length)
            .Where(g => g.Count() > 1)
            .Select(g => g.ToArray())
            .ToArray();

        var duplicates = possibleDuplicates
            .SelectMany(x => x
                .GroupBy(f => (
                    f.FileInfo.Length,
                    Hash: fileHasher.ComputeFileHash(f.FileInfo.FullName)))
                .Where(g => g.Count() > 1)
                .Select(g => g.ToArray()))
            .ToArray();

        return duplicates;
    }
}
using DuplicateFinder.Files;

namespace DuplicateFinder;

internal class DuplicateFileFinder
{
    private readonly IFileFinder _fileFinder;
    private readonly IFileHasher _fileHasher;

    public DuplicateFileFinder(
        IFileFinder fileFinder,
        IFileHasher fileHasher)
    {
        _fileFinder = fileFinder;
        _fileHasher = fileHasher;
    }

    public FileInfo[][] FindDuplications()
    {
        var allFileInfos = _fileFinder.FindFiles();

        var fileInfosBySize = allFileInfos
            .GroupBy(f => f.Length)
            .Where(g => g.Count() > 1)
            .ToList();

        var duplicatesBySizeAndHash = fileInfosBySize
            .SelectMany(g => g.Select(x => x))
            .GroupBy(f => _fileHasher.ComputeFileHash(f.FullName))
            .Where(g => g.Count() > 1)
            .ToList();

        return duplicatesBySizeAndHash.Select(g => g.ToArray()).ToArray();
    }
}
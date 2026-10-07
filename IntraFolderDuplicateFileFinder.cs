using DuplicateFinder.Files;

namespace DuplicateFinder;

internal class IntraFolderDuplicateFileFinder
{
    private readonly IFileFinder _fileFinder;
    private readonly IFileHasher _fileHasher;

    public IntraFolderDuplicateFileFinder(
        IFileFinder fileFinder,
        IFileHasher fileHasher)
    {
        _fileFinder = fileFinder;
        _fileHasher = fileHasher;
    }

    public FileInfoWrapper[][] FindDuplications()
    {
        var allFileInfos = _fileFinder.FindFiles();

        var duplicateFileInfos = allFileInfos
            .GroupBy(f => (f.FileInfo.DirectoryName, f.FileInfo.Length, Hash: _fileHasher.ComputeFileHash(f.FileInfo.FullName)))
            .Where(g => g.Count() > 1)
            .ToList();

        return duplicateFileInfos
            .Select(g => g
                .OrderByDescending(x => x.FileInfo.Name)
                .Select((x,n) => x with { Attributes = x.Attributes with { ToBeDeleted = n > 0 } })
                .ToArray())
            .ToArray();
    }
}
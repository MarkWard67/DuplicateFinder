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

    public FileInfoWrapper[][] FindDuplications()
    {
        // When a file is duplicated within a single folder, we want to ignore all but one of the duplicates
        var intraFolderDuplicateFinder = new IntraFolderDuplicateFileFinder(_fileFinder, _fileHasher);
        var intraFolderDuplicates = intraFolderDuplicateFinder.FindDuplications();
        var fileFullNamesToIgnore = intraFolderDuplicates
            .SelectMany(x => x)
            .Where(x => x.Attributes.ToBeDeleted)
            .Select(x => x.FileInfo.FullName)
            .Distinct()
            .ToHashSet();

        var allFileInfos = _fileFinder.FindFiles()
            .Where(x => !fileFullNamesToIgnore.Contains(x.FileInfo.FullName));

        var duplicateFileInfos = allFileInfos
            .GroupBy(f => (f.FileInfo.Length, Hash: _fileHasher.ComputeFileHash(f.FileInfo.FullName)))
            .Where(g => g.Count() > 1)
            .ToList();

        return duplicateFileInfos.Select(g => g.ToArray()).ToArray();
    }
}
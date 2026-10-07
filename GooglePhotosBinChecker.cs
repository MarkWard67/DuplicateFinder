using DuplicateFinder.Files;

namespace DuplicateFinder;

internal class GooglePhotosBinChecker
{
    private readonly IFileFinder _driveFileFinder;
    private readonly IFileHasher _fileHasher;
    private readonly IFileFinder _photosBinFileFinder;

    public GooglePhotosBinChecker(
        IFileFinder photosBinFileFinder,
        IFileFinder driveFileFinder,
        IFileHasher fileHasher)
    {
        _photosBinFileFinder = photosBinFileFinder;
        _driveFileFinder = driveFileFinder;
        _fileHasher = fileHasher;
    }

    public (FileInfoWrapper, FileInfoWrapper[])[] CheckBin()
    {
        var photosBinFileInfos = _photosBinFileFinder.FindFiles();
        var driveFileInfos = _driveFileFinder.FindFiles();

        var driveFileInfosBySize = driveFileInfos
            .GroupBy(x => x.FileInfo.Length)
            .ToDictionary(
                x => x.Key,
                x => x.ToArray());

        var driveFileInfosByName = driveFileInfos
            .GroupBy(x => x.FileInfo.Name, StringComparer.OrdinalIgnoreCase
            )
            .ToDictionary(
                x => x.Key,
                x => x.ToArray(),
                StringComparer.OrdinalIgnoreCase);

        var problemFiles = new List<(FileInfoWrapper, FileInfoWrapper[])>();
        foreach (var photoBinFileInfo in photosBinFileInfos)
        {
            var driveFilesOfSameLength = driveFileInfosBySize.GetValueOrDefault(photoBinFileInfo.FileInfo.Length);
            if (driveFilesOfSameLength == null)
            {
                var driveFilesOfSameName = driveFileInfosByName.GetValueOrDefault(photoBinFileInfo.FileInfo.Name);
                problemFiles.Add((photoBinFileInfo, driveFilesOfSameName?.ToArray() ?? []));

                    continue;
            }

            var photoBinFileHash = _fileHasher.ComputeFileHash(photoBinFileInfo.FileInfo.FullName);
            var driveFile =
                driveFilesOfSameLength
                    .FirstOrDefault(f => _fileHasher.ComputeFileHash(f.FileInfo.FullName) == photoBinFileHash);

            if (driveFile == null)
            {
                problemFiles.Add((photoBinFileInfo, []));
            }
        }

        return problemFiles.ToArray();
    }
}
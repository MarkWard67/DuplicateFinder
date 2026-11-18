namespace DuplicateFinder.Files.FilePairers;

internal interface IFilePairer : IDisposable
{
    IEnumerable<FileInfoPairing> PairFiles(IEnumerable<FileInfo> fileInfos);
}
namespace DuplicateFinder.Files.FilePairers;

internal interface IFilePairer
{
    IEnumerable<FileInfoPairing> PairFiles(IEnumerable<FileInfo> fileInfos);
}
namespace DuplicateFinder.FilePairers;

internal interface IFilePairer
{
    IEnumerable<FileInfoPairing> PairFiles(IEnumerable<FileInfo> fileInfos);
}
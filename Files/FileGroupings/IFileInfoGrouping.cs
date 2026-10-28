namespace DuplicateFinder.Files.FileGroupings;

internal interface IFileInfoGrouping : IEnumerable<FileInfoWrapper>
{
    public int Count { get; }
    bool ContainsAll(params IEnumerable<FileInfoWrapper> fileInfos);
}
namespace DuplicateFinder.Files.FileGroupings;

internal interface IFileInfoGrouping : IEnumerable<FileInfo>
{
    public int Count { get; }
    bool ContainsAll(params IEnumerable<FileInfo> fileInfos);

    public FileInfoGroupingKey? Key { get; }
}
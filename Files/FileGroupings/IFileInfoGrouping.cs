namespace DuplicateFinder.Files.FileGroupings;

internal interface IFileInfoGrouping : IEnumerable<FileInfo>
{
    public int Count { get; }

    public FileInfoGroupingKey? Key { get; }
}
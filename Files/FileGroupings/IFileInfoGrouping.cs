namespace DuplicateFinder.Files.FileGroupings;

internal interface IFileInfoGrouping : IEnumerable<IFileInfoTuple>
{
    public int Count { get; }

    public FileInfoGroupingKey? Key { get; }

    public IEnumerable<FileInfo> UniqueFileInfos();
}
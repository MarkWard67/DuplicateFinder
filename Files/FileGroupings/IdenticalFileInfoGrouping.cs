namespace DuplicateFinder.Files.FileGroupings;

internal record IdenticalFileInfoGrouping : FileInfoGrouping
{
    public IdenticalFileInfoGrouping(
        FileInfoGroupingKey key,
        IEnumerable<FileInfo> fileInfos)
        : base(key, fileInfos)
    {
    }

    public IdenticalFileInfoGrouping(
        FileInfoGroupingKey key,
        IEnumerable<IFileInfoTuple> fileInfoTuples)
        : base(key, fileInfoTuples)
    {
    }

    public override string ToString()
    {
        return ToString("Identical ");
    }
}
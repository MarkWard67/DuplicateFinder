namespace DuplicateFinder.Files.FileGroupings;

internal record IdenticalFileInfoGrouping : FileInfoGrouping
{
    public IdenticalFileInfoGrouping(
        FileInfoGroupingKey key,
        IEnumerable<FileInfo> fileInfos)
        : base(FileInfoGroupingType.Identical, key, fileInfos)
    {
    }

    public IdenticalFileInfoGrouping(
        FileInfoGroupingKey key,
        IFileInfoTuple tuple)
        : base(FileInfoGroupingType.Identical, key, tuple)
    {
    }

    public IdenticalFileInfoGrouping(
        FileInfoGroupingKey key,
        IEnumerable<IFileInfoTuple> fileInfoTuples)
        : base(FileInfoGroupingType.Identical, key, fileInfoTuples)
    {
    }
}
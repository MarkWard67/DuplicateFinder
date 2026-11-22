namespace DuplicateFinder.Files.FileGroupings;

internal record SimilarFileInfoGrouping : FileInfoGrouping
{
    public SimilarFileInfoGrouping(
        FileInfoGroupingKey key,
        IEnumerable<FileInfo> fileInfos)
        : base(key, fileInfos)
    {
    }

    public SimilarFileInfoGrouping(
        FileInfoGroupingKey key,
        IEnumerable<IFileInfoTuple> fileInfoTuples)
        : base(key, fileInfoTuples)
    {
    }

    public override string ToString()
    {
        return ToString("Similar ");
    }
}
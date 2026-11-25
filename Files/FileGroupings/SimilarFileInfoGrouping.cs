namespace DuplicateFinder.Files.FileGroupings;

internal record SimilarFileInfoGrouping : FileInfoGrouping
{
    public SimilarFileInfoGrouping(
        FileInfoGroupingKey key,
        IEnumerable<FileInfo> fileInfos)
        : base(FileInfoGroupingType.Similar, key, fileInfos)
    {
    }

    public SimilarFileInfoGrouping(
        FileInfoGroupingKey key,
        IEnumerable<IFileInfoTuple> fileInfoTuples)
        : base(FileInfoGroupingType.Similar, key, fileInfoTuples)
    {
    }
}
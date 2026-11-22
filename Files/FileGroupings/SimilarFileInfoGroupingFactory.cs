namespace DuplicateFinder.Files.FileGroupings;

internal class SimilarFileInfoGroupingFactory : IFileInfoGroupingFactory
{
    public static SimilarFileInfoGroupingFactory Instance { get; } = new();

    private SimilarFileInfoGroupingFactory()
    {
    }

    public IFileInfoGrouping CreateFileInfoGrouping(
        IEnumerable<FileInfo> fileInfos,
        FileInfoGroupingKey? key = null)
    {
        return new SimilarFileInfoGrouping(key ?? new FileInfoGroupingKey(), fileInfos);
    }

    public IFileInfoGrouping CreateFileInfoGrouping(
        IEnumerable<IFileInfoTuple> fileInfoTuples,
        FileInfoGroupingKey? key = null)
    {
        return new SimilarFileInfoGrouping(key ?? new FileInfoGroupingKey(), fileInfoTuples);

    }
}
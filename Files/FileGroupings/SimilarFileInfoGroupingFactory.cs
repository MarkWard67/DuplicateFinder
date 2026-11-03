namespace DuplicateFinder.Files.FileGroupings;

internal class SimilarFileInfoGroupingFactory : IFileInfoGroupingFactory
{
    public static SimilarFileInfoGroupingFactory Instance { get; } = new SimilarFileInfoGroupingFactory();

    private SimilarFileInfoGroupingFactory()
    {
    }

    public IFileInfoGrouping CreateFileInfoGrouping(
        IEnumerable<FileInfo> fileInfos,
        FileInfoGroupingKey? key = null)
    {
        return new SimilarFileInfoGrouping(fileInfos, key ?? new FileInfoGroupingKey());
    }
}
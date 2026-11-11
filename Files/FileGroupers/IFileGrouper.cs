using DuplicateFinder.Files.FileGroupings;

namespace DuplicateFinder.Files.FileGroupers;

internal interface IFileGrouper : IDisposable
{
    public IEnumerable<IFileInfoGrouping> GroupFiles(
        IEnumerable<FileInfo> fileInfos,
        FileInfoGroupingKey? key = null);

    public IEnumerable<IFileInfoGrouping> GroupFiles(
        IEnumerable<FileInfo> fileInfos,
        IFileInfoGroupingFactory groupingFactory,
        FileInfoGroupingKey? key = null);
}

internal interface ISlowFileGrouper : IFileGrouper
{
    public event EventHandler<FileGroupingHitEventArgs> FileGroupingHit;

    public event EventHandler<string> ProgressUpdate;
}
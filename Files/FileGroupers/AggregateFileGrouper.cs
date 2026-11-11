using DuplicateFinder.Files.FileGroupings;

namespace DuplicateFinder.Files.FileGroupers;

internal class AggregateFileGrouper : ISlowFileGrouper
{
    private readonly IFileInfoGroupingFactory _groupingFactory;
    private readonly IFileGrouper[] _fileGroupers;

    public AggregateFileGrouper(
        IFileInfoGroupingFactory groupingFactory,
        params IFileGrouper[] fileGroupers)
    {
        _groupingFactory = groupingFactory;
        _fileGroupers = fileGroupers;

        var slowFileGroupers = fileGroupers.OfType<ISlowFileGrouper>();
        foreach (var slowFileGrouper in slowFileGroupers)
        {
            slowFileGrouper.FileGroupingHit += (sender, args) => RaiseFileGroupingHit(args.FileGrouping, sender);
            slowFileGrouper.ProgressUpdate += (sender, progressUpdate) =>
            {
                RaiseProgressUpdate(progressUpdate, sender);
            };
        }
    }

    public IEnumerable<IFileInfoGrouping> GroupFiles(
        IEnumerable<FileInfo> fileInfos,
        FileInfoGroupingKey? key = null)
    {
        return GroupFiles(
            fileInfos,
            _groupingFactory,
            key);
    }

    public IEnumerable<IFileInfoGrouping> GroupFiles(
        IEnumerable<FileInfo> fileInfos,
        IFileInfoGroupingFactory groupingFactoryOverride,
        FileInfoGroupingKey? key = null)
    {
        var result = new List<IFileInfoGrouping> { new FileInfoGrouping(fileInfos, key) };

        foreach (var fileGrouper in _fileGroupers)
        {
            var ret = new List<IFileInfoGrouping>();
            foreach (var grouping in result)
            {
                ret.AddRange(
                    fileGrouper.GroupFiles(
                        grouping,
                        groupingFactoryOverride,
                        grouping.Key));
            }

            result = ret;
        }

        return result.ToArray();
    }

    public event EventHandler<FileGroupingHitEventArgs>? FileGroupingHit;
    public event EventHandler<string>? ProgressUpdate;

    private void RaiseFileGroupingHit(
        IFileInfoGrouping fileInfoGrouping,
        object? sender = null)
    {
        FileGroupingHit?.Invoke(sender ?? this, new FileGroupingHitEventArgs(fileInfoGrouping));
    }

    private void RaiseProgressUpdate(
        string progressUpdate,
        object? sender = null)
    {
        ProgressUpdate?.Invoke(sender ?? this, progressUpdate);
    }

    public void Dispose()
    {
        // TODO release managed resources here
    }
}
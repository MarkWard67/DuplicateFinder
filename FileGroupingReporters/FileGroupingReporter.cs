using System.Text;
using DuplicateFinder.Files.FileFinders;
using DuplicateFinder.Files.FileGroupers;
using DuplicateFinder.Files.FileGroupings;

namespace DuplicateFinder.FileGroupingReporters;

internal abstract class FileGroupingReporter : IFileGroupingReporter
{
    private readonly IFileFinder _fileFinder;
    private readonly IFileGrouper _fileGrouper;

    protected FileGroupingReporter(
        IFileFinder fileFinder,
        IFileGrouper fileGrouper)
    {
        _fileFinder = fileFinder;
        _fileGrouper = fileGrouper;

        if (fileGrouper is ISlowFileGrouper slowFileGrouper)
        {
            slowFileGrouper.FileGroupingHit += OnFileGroupingHit;
            slowFileGrouper.ProgressUpdate += OnProgressUpdate;
        }
    }

    public string Report()
    {
        var fileGroups = GroupFiles();
        return Report(fileGroups);
    }

    public event EventHandler<SubReportEventArgs>? SubReportAvailable;
    public event EventHandler<string>? ProgressUpdate;

    private void OnFileGroupingHit(object? sender, FileGroupingHitEventArgs e)
    {
        var report = Report([e.FileGrouping]);
        RaiseSubReportAvailable(report);
    }
    
    private void OnProgressUpdate(object? sender, string e)
    {
        ProgressUpdate?.Invoke(sender ?? this, e);
    }

    private void RaiseSubReportAvailable(string subReport)
    {
        if (subReport == string.Empty)
        {
            return;
        }

        SubReportAvailable?.Invoke(
            this,
            new SubReportEventArgs(subReport));
    }

    protected virtual IEnumerable<IFileInfoGrouping> GroupFiles()
    {
        var fileInfos = _fileFinder.FindFiles();
        var fileGroups = _fileGrouper.GroupFiles(fileInfos);

        return fileGroups;
    }

    private string Report(IEnumerable<IFileInfoGrouping> fileGroups)
    {
       var sb = new StringBuilder();
        foreach (var fileGroup in GetReportableFileGroups(fileGroups))
        {
            sb.AppendLine($"{fileGroup}");
        }

        return sb.ToString();
    }

    protected virtual IEnumerable<IFileInfoGrouping> GetReportableFileGroups(IEnumerable<IFileInfoGrouping> fileGroups)
    {
        return fileGroups.Where(x => x.Count > 1);
    }
}
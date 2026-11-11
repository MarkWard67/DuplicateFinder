namespace DuplicateFinder.FileGroupingReporters;

internal interface IFileGroupingReporter : IDisposable
{
    string Report();

    public event EventHandler<SubReportEventArgs> SubReportAvailable;

    public event EventHandler<string> ProgressUpdate;
}
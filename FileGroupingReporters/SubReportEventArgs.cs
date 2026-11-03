namespace DuplicateFinder.FileGroupingReporters;

internal class SubReportEventArgs(string subReport) : EventArgs
{
    public string SubReport { get; } = subReport;
}
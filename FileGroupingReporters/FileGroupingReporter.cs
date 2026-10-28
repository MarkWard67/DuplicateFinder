using System.Text;
using DuplicateFinder.Files.FileFinders;
using DuplicateFinder.Files.FileGroupers;
using DuplicateFinder.Files.FileGroupings;

namespace DuplicateFinder.FileGroupingReporters;

internal abstract class FileGroupingReporter(
    IFileFinder fileFinder,
    IFileGrouper fileGrouper) : IFileGroupingReporter
{
    public string Report()
    {
        var fileGroups = GroupFiles();

        var sb = new StringBuilder();
        foreach (var fileGroup in fileGroups) sb.AppendLine($"{fileGroup}");

        return sb.ToString();
    }

    protected virtual IEnumerable<IFileInfoGrouping> GroupFiles()
    {
        var fileInfos = fileFinder.FindFiles();
        var fileGroups = fileGrouper.GroupFiles(fileInfos);

        var similarFiles = fileGroups
            .Where(x => x.Count > 1);

        return similarFiles;
    }
}
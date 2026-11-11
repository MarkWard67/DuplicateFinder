using DuplicateFinder.Files.FileFinders;
using DuplicateFinder.Files.FileGroupers;
using DuplicateFinder.Files.FileGroupings;
using DuplicateFinder.Files.FileHashers;
using DuplicateFinder.Reporters;

namespace DuplicateFinder.FileGroupingReporters;

internal class DuplicatesFileGroupingReporter(
    IFileFinder fileFinder,
    IFileGrouper outermostFileGrouper,
    IFileHasher fileHasher,
    IReporter reporter) : FileGroupingReporter(
    fileFinder,
    new AggregateFileGrouper(
        IdenticalFileInfoGroupingFactory.Instance,
        outermostFileGrouper,
        FileLengthFileGrouper.Instance,
        new FileHashFileGrouper(fileHasher)),
    reporter)
{
    protected override IEnumerable<IFileInfoGrouping> GroupFiles()
    {
        var result = base.GroupFiles();
        return result.Where(x => x.Count > 1);
    }
}
using DuplicateFinder.Files;
using DuplicateFinder.Files.FileFinders;
using DuplicateFinder.Files.FileGroupers;
using DuplicateFinder.Files.FileGroupings;

namespace DuplicateFinder.FileGroupingReporters;

internal class DuplicatesFileGroupingReporter(
    IFileFinder fileFinder,
    IFileGrouper outermostFileGrouper,
    IFileHasher fileHasher) : FileGroupingReporter(
    fileFinder,
    new AggregateFileGrouper(
        IdenticalFileInfoGroupingFactory.Instance,
        outermostFileGrouper,
        FileLengthFileGrouper.Instance,
        new FileHashFileGrouper(fileHasher)))
{
    protected override IEnumerable<IFileInfoGrouping> GroupFiles()
    {
        var result = base.GroupFiles();
        return result.Where(x => x.Count > 1);
    }
}
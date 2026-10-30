using DuplicateFinder.Files;
using DuplicateFinder.Files.FileFinders;
using DuplicateFinder.Files.FileGroupers;
using DuplicateFinder.Files.FileGroupings;

namespace DuplicateFinder.FileGroupingReporters;

internal class DuplicatesFileGroupingReporter(
    IFileFinder fileFinder,
    IFileGrouper fileGrouper,
    IFileHasher fileHasher) : FileGroupingReporter(
    fileFinder,
    new AggregateFileGrouper(
        IdenticalFileInfoGroupingFactory.Instance,
        fileGrouper,
        FileLengthFileGrouper.Instance,
        new FileHashFileGrouper(fileHasher)));
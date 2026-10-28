using DuplicateFinder.Files;
using DuplicateFinder.Files.FileFinders;
using DuplicateFinder.Files.FileGroupers;

namespace DuplicateFinder.FileGroupingReporters;

internal class SimilarFilesFileGroupingReporter(
    IFileFinder fileFinder,
    IFileGrouper fileGrouper,
    IFileHasher fileHasher) : FileGroupingReporter(
    fileFinder,
    new AggregateFileGrouper(
        fileGrouper,
        new SimilarImagesFileGrouper(fileHasher)));
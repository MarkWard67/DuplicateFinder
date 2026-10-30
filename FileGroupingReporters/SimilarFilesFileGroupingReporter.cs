using DuplicateFinder.Files;
using DuplicateFinder.Files.FileFinders;
using DuplicateFinder.Files.FileGroupers;
using DuplicateFinder.Files.FileGroupings;

namespace DuplicateFinder.FileGroupingReporters;

internal class SimilarFilesFileGroupingReporter(
    IFileFinder fileFinder,
    IFileGrouper fileGrouper,
    IFileHasher fileHasher,
    IImageComparer imageComparer) : FileGroupingReporter(
    fileFinder,
    new AggregateFileGrouper(
        SimilarFileInfoGroupingFactory.Instance,
        fileGrouper,
        new SimilarImagesFileGrouper(fileHasher, imageComparer)));
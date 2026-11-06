using DuplicateFinder.FilePairers;
using DuplicateFinder.Files;
using DuplicateFinder.Files.FileFinders;
using DuplicateFinder.Files.FileGroupers;
using DuplicateFinder.Files.FileGroupings;
using DuplicateFinder.Reporters;

namespace DuplicateFinder.FileGroupingReporters;

internal class SimilarFilesFileGroupingReporter(
    IFileFinder fileFinder,
    IFileGrouper fileGrouper,
    IFileHasher fileHasher,
    IImageComparer imageComparer,
    IReporter reporter,
    IFilePairer filePairer) : FileGroupingReporter(
    fileFinder,
    new AggregateFileGrouper(
        SimilarFileInfoGroupingFactory.Instance,
        fileGrouper,
        new SimilarImagesFileGrouper(fileHasher, imageComparer, filePairer)),
    reporter);
using DuplicateFinder.Reporters;

namespace DuplicateFinder.Files.FileGroupings;

internal record FileInfo2Tuple(
    FileInfo FileInfo1,
    FileInfo FileInfo2,
    ICsvReportable? Properties = null)
    : FileInfoNTuple([FileInfo1, FileInfo2], Properties);

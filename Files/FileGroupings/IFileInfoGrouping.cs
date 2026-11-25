using DuplicateFinder.Reporters;

namespace DuplicateFinder.Files.FileGroupings;

internal interface IFileInfoGrouping : IEnumerable<IFileInfoTuple>, ICsvReportable
{
    public FileInfoGroupingKey? Key { get; }

    public IEnumerable<FileInfo> UniqueFileInfos();

    public FileInfoGroupingType Type { get; }
}
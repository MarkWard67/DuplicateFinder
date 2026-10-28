using DuplicateFinder.Files.FileGroupings;

namespace DuplicateFinder.Files.FileGroupers;

internal interface IFileGrouper
{
    public IEnumerable<IFileInfoGrouping> GroupFiles(IEnumerable<FileInfoWrapper> fileInfos);
}
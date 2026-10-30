namespace DuplicateFinder.Files.FileGroupings;

internal interface IFileInfoGroupingFactory
{
    IFileInfoGrouping CreateFileInfoGrouping(
        IEnumerable<FileInfoWrapper> fileInfos,
        FileInfoGroupingKey? key = null);
}
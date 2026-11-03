namespace DuplicateFinder.Files.FileGroupings;

internal interface IFileInfoGroupingFactory
{
    IFileInfoGrouping CreateFileInfoGrouping(
        IEnumerable<FileInfo> fileInfos,
        FileInfoGroupingKey? key = null);
}
namespace DuplicateFinder.Files;

internal record FileInfoWrapper
{
    public FileInfoWrapper(
        FileInfo fileInfo,
        FileInfoAttributes? attributes = null)
    {
        FileInfo = fileInfo;
        Attributes = attributes ?? new FileInfoAttributes();
    }

    public FileInfo FileInfo { get; }

    public FileInfoAttributes Attributes { get; init; }
}
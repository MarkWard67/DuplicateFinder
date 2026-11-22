namespace DuplicateFinder.Files.FileGroupings;

internal record FileInfo2Tuple(
    FileInfo FileInfo1,
    FileInfo FileInfo2,
    object? Properties = null)
    : IFileInfoTuple
{
    public int Length { get; } = 2;

    public FileInfo this[int index] => index switch
    {
        0 => FileInfo1,
        1 => FileInfo2,
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };
}
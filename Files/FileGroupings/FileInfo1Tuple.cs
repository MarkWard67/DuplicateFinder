namespace DuplicateFinder.Files.FileGroupings;

internal record FileInfo1Tuple(
    FileInfo FileInfo1,
    object? Properties = null) : IFileInfoTuple
{
    public int Length { get; } = 1;

    public FileInfo this[int index] => index == 0 ? FileInfo1 : throw new ArgumentOutOfRangeException(nameof(index));
}
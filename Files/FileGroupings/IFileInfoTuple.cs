namespace DuplicateFinder.Files.FileGroupings;

internal interface IFileInfoTuple
{
    public int Length { get; }

    /// <summary>
    ///     Get the element at position
    ///     <param name="index" />
    ///     .
    /// </summary>
    public FileInfo this[int index] { get; }

    public object? Properties { get; }
}
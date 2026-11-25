using DuplicateFinder.Reporters;

namespace DuplicateFinder.Files.FileGroupings;

internal interface IFileInfoTuple : ICsvReportable, IEnumerable<FileInfo>
{
    public int Length { get; }

    /// <summary>
    ///     Get the element at position
    ///     <param name="index" />
    ///     .
    /// </summary>
    public FileInfo this[int index] { get; }

    public ICsvReportable? Properties { get; }
}
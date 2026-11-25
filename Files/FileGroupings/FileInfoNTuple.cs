using System.Collections;
using DuplicateFinder.Reporters;

namespace DuplicateFinder.Files.FileGroupings;

internal record FileInfoNTuple : IFileInfoTuple
{
    private readonly IReadOnlyCollection<FileInfo> _fileInfos;

    public FileInfoNTuple(
        IEnumerable<FileInfo> FileInfos,
        ICsvReportable? Properties = null)
    {
        _fileInfos = FileInfos.ToList();
        this.Properties = Properties;
    }

    public IEnumerable<FileInfo> FileInfos => _fileInfos;

    public int Length => _fileInfos.Count;
    public ICsvReportable? Properties { get; init; }

    public FileInfo this[int index]
    {
        get
        {
            if (index < 0 || index >= Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return FileInfos.ElementAt(index);
        }
    }

    public string ToCsv(char separator = CsvValues.Separator, char delimiter = CsvValues.Separator)
    {
        return this.ToCsvImpl(separator, delimiter);
    }

    public IEnumerator<FileInfo> GetEnumerator()
    {
        return _fileInfos.GetEnumerator();
    }

    public override string ToString()
    {
        return ToCsv();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
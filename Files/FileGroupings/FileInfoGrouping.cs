using System.Collections;
using DuplicateFinder.Reporters;

namespace DuplicateFinder.Files.FileGroupings;

internal record FileInfoGrouping : IFileInfoGrouping
{
    private readonly IReadOnlyCollection<IFileInfoTuple> _fileInfoTuples;

    public FileInfoGrouping(
        FileInfoGroupingType type,
        FileInfoGroupingKey? key,
        IEnumerable<FileInfo>? fileInfos = null)
        : this(type, key)
    {
        _fileInfoTuples = fileInfos?
            .Select(x => new FileInfo1Tuple(x))
            .Cast<IFileInfoTuple>()
            .ToList() ?? [];
    }

    public FileInfoGrouping(
        FileInfoGroupingType type,
        FileInfoGroupingKey? key,
        IFileInfoTuple tuple)
        : this(type, key)
    {
        _fileInfoTuples = [tuple];
    }

    public FileInfoGrouping(
        FileInfoGroupingType type,
        FileInfoGroupingKey? key,
        IEnumerable<IFileInfoTuple>? fileInfoTuples = null)
        : this(type, key)
    {
        _fileInfoTuples = fileInfoTuples?.ToList() ?? [];
    }

    private FileInfoGrouping(
        FileInfoGroupingType type,
        FileInfoGroupingKey? key)
    {
        Type = type;
        Key = key;
        _fileInfoTuples = [];
    }

    public int Count => _fileInfoTuples.Count;

    public IEnumerator<IFileInfoTuple> GetEnumerator()
    {
        return _fileInfoTuples.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public FileInfoGroupingKey? Key { get; }

    public FileInfoGroupingType Type { get; }

    public IEnumerable<FileInfo> UniqueFileInfos()
    {
        return _fileInfoTuples
            .SelectMany(x => Enumerable.Range(0, x.Length).Select(y => x[y]))
            .Select(x => x)
            .DistinctBy(x => x.FullName);
    }

    public virtual string ToCsv(char separator = CsvValues.Separator, char delimiter = CsvValues.Delimiter)
    {
        var values = new List<object> { Type };
        values.AddRange(_fileInfoTuples);

        return CsvReporter.ToCsv(
            values,
            separator,
            delimiter);
    }

    public override string ToString()
    {
        return ToCsv();
    }
}
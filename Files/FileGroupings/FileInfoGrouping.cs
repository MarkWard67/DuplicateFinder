using System.Collections;
using System.Text;

namespace DuplicateFinder.Files.FileGroupings;

internal record FileInfoGrouping : IFileInfoGrouping
{
    private readonly List<IFileInfoTuple> _fileInfoTuples;

    public FileInfoGrouping(
        FileInfoGroupingKey? key,
        IEnumerable<FileInfo>? fileInfos = null)
    {
        Key = key;
        _fileInfoTuples = fileInfos?
            .Select(x => new FileInfo1Tuple(x))
            .Cast<IFileInfoTuple>()
            .ToList() ?? [];
    }

    public FileInfoGrouping(
        FileInfoGroupingKey? key,
        IEnumerable<IFileInfoTuple>? fileInfoTuples = null)
    {
        Key = key;
        _fileInfoTuples = fileInfoTuples?.ToList() ?? [];
    }

    public IEnumerator<IFileInfoTuple> GetEnumerator()
    {
        return _fileInfoTuples.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public int Count => _fileInfoTuples.Count;

    public FileInfoGroupingKey? Key { get; }

    public IEnumerable<FileInfo> UniqueFileInfos()
    {
        return _fileInfoTuples
            .SelectMany(x => Enumerable.Range(0, x.Length).Select(y => x[y]))
            .Select(x => x)
            .DistinctBy(x => x.FullName);
    }

    public override string ToString()
    {
        return ToString(string.Empty);
    }

    protected string ToString(string title)
    {
        var sb = new StringBuilder($"{title}{Key}").AppendLine();

        foreach (var fileInfoTuple in _fileInfoTuples)
        {
            if (fileInfoTuple.Properties != null)
            {
                sb.AppendLine($"\t{fileInfoTuple.Properties}");
            }

            for (var i = 0; i < fileInfoTuple.Length; i++)
            {
                var fileInfo = fileInfoTuple[i];
                sb.AppendLine($"\t{fileInfo.FullName}");
            }
        }

        return sb.ToString();
    }
}
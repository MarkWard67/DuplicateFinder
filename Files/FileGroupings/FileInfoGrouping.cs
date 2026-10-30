using System.Collections;
using System.Text;

namespace DuplicateFinder.Files.FileGroupings;

internal record FileInfoGrouping : IFileInfoGrouping
{
    private readonly List<FileInfoWrapper> _fileInfos;

    public FileInfoGrouping(
        IEnumerable<FileInfoWrapper> fileInfos,
        FileInfoGroupingKey? key)
    {
        Key = key;
        _fileInfos = [.. fileInfos];
    }

    public IEnumerator<FileInfoWrapper> GetEnumerator()
    {
        return _fileInfos.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public int Count => _fileInfos.Count;

    public bool ContainsAll(params IEnumerable<FileInfoWrapper> fileInfos)
    {
        return fileInfos.All(_fileInfos.Contains);
    }

    public FileInfoGroupingKey? Key { get; }

    public override string ToString()
    {
        var sb = new StringBuilder($"{Key}").AppendLine();

        foreach (var fileInfo in _fileInfos)
        {
            sb.AppendLine(fileInfo.FileInfo.FullName);
        }

        return sb.ToString();
    }
}
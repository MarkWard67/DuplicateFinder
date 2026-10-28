using System.Collections;
using System.Text;

namespace DuplicateFinder.Files.FileGroupings;

internal record FileInfoGrouping : IFileInfoGrouping
{
    private readonly List<FileInfoWrapper> _fileInfos;

    public FileInfoGrouping(IEnumerable<FileInfoWrapper> fileInfos)
    {
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

    public override string ToString()
    {
        var sb = new StringBuilder();

        foreach (var fileInfo in _fileInfos) sb.AppendLine(fileInfo.FileInfo.FullName);

        return sb.ToString();
    }
}
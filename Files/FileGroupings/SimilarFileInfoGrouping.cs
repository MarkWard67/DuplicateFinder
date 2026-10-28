using System.Collections;
using System.Text;

namespace DuplicateFinder.Files.FileGroupings;

internal record SimilarFileInfoGrouping : IFileInfoGrouping
{
    private readonly FileInfoWrapper _baseFileInfo;
    private readonly List<FileInfoWrapper> _similarFileInfos;

    public SimilarFileInfoGrouping(
        FileInfoWrapper baseFileInfo,
        IEnumerable<FileInfoWrapper> similarFileInfos)
    {
        _baseFileInfo = baseFileInfo;
        _similarFileInfos = similarFileInfos.ToList();
    }

    public IEnumerator<FileInfoWrapper> GetEnumerator()
    {
        yield return _baseFileInfo;

        foreach (var similarFileInfo in _similarFileInfos) yield return similarFileInfo;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public int Count => _similarFileInfos.Count + 1;

    public override string ToString()
    {
        var sb = new StringBuilder($"Similar to {_baseFileInfo.FileInfo.FullName}:").AppendLine();

        foreach (var fileInfo in _similarFileInfos) sb.AppendLine($"\t{fileInfo.FileInfo.FullName}");

        return sb.ToString();
    }
}
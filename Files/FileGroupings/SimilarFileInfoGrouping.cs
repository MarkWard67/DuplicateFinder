using System.Text;

namespace DuplicateFinder.Files.FileGroupings;

internal record SimilarFileInfoGrouping : FileInfoGrouping
{
    public SimilarFileInfoGrouping(
        IEnumerable<FileInfo> fileInfos,
        FileInfoGroupingKey key)
        : base(fileInfos, key)
    {
    }

    public override string ToString()
    {
        var sb = new StringBuilder($"Similar ({Key})").AppendLine();

        var i = 1;
        foreach (var fileInfo in this.OrderBy(x => x.Length).ThenBy(x => x.FullName.Length))
        {
            sb.AppendLine($"{i++}.\t{fileInfo.FullName}");
        }

        return sb.ToString();
    }
}
using System.Text;

namespace DuplicateFinder.Files.FileGroupings;

internal record IdenticalFileInfoGrouping : FileInfoGrouping
{
    public IdenticalFileInfoGrouping(
        IEnumerable<FileInfo> fileInfos,
        FileInfoGroupingKey key)
        : base(fileInfos, key)
    {
    }

    public override string ToString()
    {
        var sb = new StringBuilder($"Identical ({Key})").AppendLine();

        var i = 1;
        foreach (var fileInfo in this.OrderBy(x => x.FullName.Length))
        {
            sb.AppendLine($"{i++}.\t{fileInfo.FullName}");
        }

        return sb.ToString();
    }
}
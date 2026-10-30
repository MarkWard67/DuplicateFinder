using System.Text;

namespace DuplicateFinder.Files.FileGroupings;

internal record IdenticalFileInfoGrouping : FileInfoGrouping
{
    public IdenticalFileInfoGrouping(
        IEnumerable<FileInfoWrapper> fileInfos,
        FileInfoGroupingKey key)
        : base(fileInfos, key)
    {
    }

    public override string ToString()
    {
        var sb = new StringBuilder($"Identical ({Key})").AppendLine();

        foreach (var fileInfo in this)
        {
            sb.AppendLine($"\t{fileInfo.FileInfo.FullName}");
        }

        return sb.ToString();
    }
}
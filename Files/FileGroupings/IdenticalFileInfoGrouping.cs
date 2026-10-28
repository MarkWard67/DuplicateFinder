using System.Text;

namespace DuplicateFinder.Files.FileGroupings;

internal record IdenticalFileInfoGrouping : FileInfoGrouping
{
    private readonly string _fileHash;
    private readonly long _fileLength;

    public IdenticalFileInfoGrouping(
        long fileLength,
        string fileHash,
        IEnumerable<FileInfoWrapper> fileInfos)
        : base(fileInfos)
    {
        _fileLength = fileLength;
        _fileHash = fileHash;
    }

    public override string ToString()
    {
        var sb = new StringBuilder($"Identical [{_fileLength}, {_fileHash}]").AppendLine();

        foreach (var fileInfo in this) sb.AppendLine($"\t{fileInfo.FileInfo.FullName}");

        return sb.ToString();
    }
}
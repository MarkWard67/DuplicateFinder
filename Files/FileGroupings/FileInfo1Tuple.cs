using DuplicateFinder.Reporters;

namespace DuplicateFinder.Files.FileGroupings;

internal record FileInfo1Tuple : FileInfoNTuple
{
    public FileInfo1Tuple(FileInfo FileInfo1,
        ICsvReportable? Properties = null) : base([FileInfo1], Properties)
    {
        this.FileInfo1 = FileInfo1;
    }

    public FileInfo FileInfo1 { get; init; }

    public void Deconstruct(out FileInfo FileInfo1, out ICsvReportable? Properties)
    {
        FileInfo1 = this.FileInfo1;
        Properties = this.Properties;
    }
}
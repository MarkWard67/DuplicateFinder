namespace DuplicateFinder.Files.FilePairers;

internal class FileNameFilePairer : FilePairer
{
    public FileNameFilePairer()
    {
    }

    public FileNameFilePairer(string redundantFilesFolderPath)
        : base(redundantFilesFolderPath)
    {
    }

    protected override bool ShouldPair(FileInfo fileInfo1, FileInfo fileInfo2)
    {
        return fileInfo1.Name.Equals(fileInfo2.Name, StringComparison.OrdinalIgnoreCase);
    }
}
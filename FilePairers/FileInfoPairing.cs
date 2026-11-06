namespace DuplicateFinder.FilePairers;

internal record FileInfoPairing
{
    public FileInfoPairing(
        FileInfo MasterFileInfo,
        IEnumerable<FileInfo> pairedFileInfos)
    {
        this.MasterFileInfo = MasterFileInfo;
        this.PairedFileInfos = pairedFileInfos.ToArray();
    }

    public FileInfo MasterFileInfo { get; init; }
    public FileInfo[] PairedFileInfos { get; init; }

    public void Deconstruct(
        out FileInfo masterFileInfo,
        out FileInfo[] pairedFileInfos)
    {
        masterFileInfo = this.MasterFileInfo;
        pairedFileInfos = this.PairedFileInfos;
    }
}
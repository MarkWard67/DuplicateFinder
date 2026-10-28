namespace DuplicateFinder.Files;

internal record FileInfoWrapper : IEqualityComparer<FileInfoWrapper>
{
    public FileInfoWrapper(
        FileInfo fileInfo,
        FileInfoAttributes? attributes = null)
    {
        FileInfo = fileInfo;
        Attributes = attributes ?? new FileInfoAttributes();
    }

    public FileInfo FileInfo { get; }

    public FileInfoAttributes Attributes { get; init; }

    public bool Equals(FileInfoWrapper? x, FileInfoWrapper? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.FileInfo.FullName.Equals(y.FileInfo.FullName, StringComparison.OrdinalIgnoreCase);
    }

    public int GetHashCode(FileInfoWrapper obj)
    {
        return obj.FileInfo.GetHashCode();
    }
}
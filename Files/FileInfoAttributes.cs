namespace DuplicateFinder.Files;

internal record FileInfoAttributes
{
    public bool ToBeDeleted { get; init; }

    public string? FileHash { get; set; }
}
using DuplicateFinder.Files.FileGroupings;

namespace DuplicateFinder.Files.FileGroupers;

internal class FileGroupingHitEventArgs(IFileInfoGrouping fileInfoGrouping) : EventArgs
{
    public IFileInfoGrouping FileGrouping { get; } = fileInfoGrouping;
}
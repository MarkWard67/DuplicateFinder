namespace DuplicateFinder.Folders;

internal class GooglePhotosAlbumFolderExcluder(IFolderExcluder? innerExcluder = null) : IFolderExcluder
{
    public bool ExcludeFolder(string folderFullPath)
    {
        if (folderFullPath.Contains("Google Photos", StringComparison.InvariantCultureIgnoreCase) &&
            File.Exists(Path.Combine(folderFullPath, "metadata.json"))) return true;

        return innerExcluder?.ExcludeFolder(folderFullPath) == true;
    }
}
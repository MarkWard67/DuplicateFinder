namespace DuplicateFinder.Folders;

internal class GooglePhotosAlbumFolderExcluder() : IFolderExcluder
{
    public bool ExcludeFolder(string folderFullPath)
    {
        return folderFullPath.Contains("Google Photos", StringComparison.InvariantCultureIgnoreCase) &&
               File.Exists(Path.Combine(folderFullPath, "metadata.json"));
    }
}
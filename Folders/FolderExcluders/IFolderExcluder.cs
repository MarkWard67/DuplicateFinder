namespace DuplicateFinder.Folders.FolderExcluders;

internal interface IFolderExcluder
{
    bool ExcludeFolder(string folderFullPath);
}
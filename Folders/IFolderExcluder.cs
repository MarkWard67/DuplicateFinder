namespace DuplicateFinder.Folders;

internal interface IFolderExcluder
{
    bool ExcludeFolder(string folderFullPath);
}
namespace DuplicateFinder.Folders.FolderFinders;

internal interface IFolderFinder
{
    DirectoryInfo[] FindFolders();
}
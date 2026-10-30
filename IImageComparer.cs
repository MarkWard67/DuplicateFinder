namespace DuplicateFinder;

internal interface IImageComparer : IDisposable
{
    bool Compare(string image1FullName, string image2FullName);
}
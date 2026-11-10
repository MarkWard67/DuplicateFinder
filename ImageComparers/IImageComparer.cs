namespace DuplicateFinder.ImageComparers;

internal interface IImageComparer : IDisposable
{
    Task<bool> CompareAsync(string image1FullName, string image2FullName, CancellationToken cancellationToken);
}
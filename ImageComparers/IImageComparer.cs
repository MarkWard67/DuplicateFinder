namespace DuplicateFinder.ImageComparers;

internal interface IImageComparer : IDisposable
{
    Task<ImageComparisonResult> CompareAsync(string image1FullName, string image2FullName, CancellationToken cancellationToken);
}
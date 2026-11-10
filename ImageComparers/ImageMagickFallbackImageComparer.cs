using DuplicateFinder.Caches;
using ImageMagick;

namespace DuplicateFinder.ImageComparers;

internal class ImageMagickFallbackImageComparer(
    double maxDistortion,
    ICache<string, MagickImage> cache) : IImageComparer
{
    private readonly ImageMagickFastImageComparer _fastComparer = new(maxDistortion, cache);
    private readonly ImageMagickSlowImageComparer _slowComparer = new(maxDistortion, cache);

    public void Dispose()
    {
        _fastComparer.Dispose();
        _slowComparer.Dispose();
    }

    public async Task<bool> CompareAsync(string image1FullName, string image2FullName, CancellationToken cancellationToken)
    {
        if (await _fastComparer.CompareAsync(image1FullName, image2FullName, cancellationToken))
        {
            return true;
        }

        return Path.GetFileName(image1FullName).Equals(Path.GetFileName(image2FullName)) &&
               await _slowComparer.CompareAsync(image1FullName, image2FullName, cancellationToken);
    }
}
using DuplicateFinder.Caches;
using ImageMagick;

namespace DuplicateFinder.ImageComparers;

internal class ImageMagickFallbackImageComparer(
    ImageMagickImageComparer fastComparer,
    ImageMagickImageComparer slowComparer)
    : IImageComparer
{
    private readonly ImageMagickFastImageComparer _fastComparer = (ImageMagickFastImageComparer)fastComparer;
    private readonly ImageMagickSlowImageComparer _slowComparer = (ImageMagickSlowImageComparer)slowComparer;

    private ImageMagickFallbackImageComparer(
        double maxDistortionSameFileName,
        double maxDistortionDifferentFileNames,
        ICache<string, MagickImage> cache)
    : this(
        new ImageMagickFastImageComparer(maxDistortionSameFileName, maxDistortionDifferentFileNames, cache),
        new ImageMagickSlowImageComparer(maxDistortionSameFileName, maxDistortionDifferentFileNames, cache))
    {
    }

    public void Dispose()
    {
        _fastComparer.Dispose();
        _slowComparer.Dispose();
    }

    public async Task<ImageComparisonResult> CompareAsync(string image1FullName, string image2FullName, CancellationToken cancellationToken)
    {
        var fastComparisonResult = await _fastComparer.CompareAsync(image1FullName, image2FullName, cancellationToken);
        if (fastComparisonResult.Type.ImagesMatch())
        {
            return fastComparisonResult;
        }

        var baseName1 = Path.GetFileNameWithoutExtension(image1FullName);
        var baseName2 = Path.GetFileNameWithoutExtension(image2FullName);
        if (baseName1.Contains(baseName2, StringComparison.OrdinalIgnoreCase) ||
            baseName2.Contains(baseName1, StringComparison.OrdinalIgnoreCase))
        {
            return await _slowComparer.CompareAsync(image1FullName, image2FullName, cancellationToken);
        }

        return fastComparisonResult;
    }
}
using DuplicateFinder.Caches;
using ImageMagick;

namespace DuplicateFinder.ImageComparers;

internal class ImageMagickSlowImageComparer(
    double maxDistortion,
    ICache<string, MagickImage> cache)
    : ImageMagickImageComparer(maxDistortion,
        cache,
        ErrorMetric.PerceptualHash,
        Channels.Red | Channels.Green | Channels.Blue)
{
    protected override List<int> GetRotationAngles(MagickImage image1, MagickImage image2)
    {
        return [0];
    }
}
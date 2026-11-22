using DuplicateFinder.Caches;
using ImageMagick;

namespace DuplicateFinder.ImageComparers;

internal class ImageMagickFastImageComparer(
    double maxDistortionSameFileName,
    double maxDistortionDifferentFileNames,
    ICache<string, MagickImage> cache) : ImageMagickImageComparer(
    maxDistortionSameFileName,
    maxDistortionDifferentFileNames,
    cache,
    ErrorMetric.MeanAbsolute,
    Channels.Red | Channels.Green | Channels.Blue | Channels.Alpha)
{
    protected override void DecorateImage(MagickImage image)
    {
        image.ColorFuzz = new Percentage(10);
    }
}
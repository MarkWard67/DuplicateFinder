using DuplicateFinder.Caches;
using ImageMagick;

namespace DuplicateFinder.ImageComparers;

internal class ImageMagickFastImageComparer(
    double maxDistortion,
    ICache<string, MagickImage> cache) : ImageMagickImageComparer(
    maxDistortion,
    cache,
    ErrorMetric.MeanAbsolute,
    Channels.Red | Channels.Green | Channels.Blue | Channels.Alpha);
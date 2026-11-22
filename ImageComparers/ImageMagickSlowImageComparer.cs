using DuplicateFinder.Caches;
using ImageMagick;

namespace DuplicateFinder.ImageComparers;

internal class ImageMagickSlowImageComparer(
    double maxDistortionSameFileName,
    double maxDistortionDifferentFileNames,
    ICache<string, MagickImage> cache)
    : ImageMagickImageComparer(
        maxDistortionSameFileName,
        maxDistortionDifferentFileNames,
        cache,
        ComparisonErrorMetric,
        Channels.Red | Channels.Green | Channels.Blue)
{
    private const ErrorMetric ComparisonErrorMetric = ErrorMetric.PerceptualHash;
    private readonly PerceptualHashCache _cache = new();

    protected override void DecorateImage(MagickImage image)
    {
        // No decoration necessary
    }

    protected override List<int> GetRotationAngles(MagickImage image1, MagickImage image2)
    {
        return [0];
    }

    protected override double CalculateDistortion(MagickImage image1, MagickImage image2, ErrorMetric errorMetric)
    {
        if (errorMetric != ErrorMetric.PerceptualHash)
        {
            return base.CalculateDistortion(image1, image2, errorMetric);
        }

        var perceptualHashTask1 = _cache.GetOrCreateAsync(
            image1.FileName!,
            _ => Task.FromResult(image1.PerceptualHash(image1.ColorSpace)),
            CancellationToken.None);

        var perceptualHashTask2 = _cache.GetOrCreateAsync(
            image2.FileName!,
            _ => Task.FromResult(image2.PerceptualHash(image2.ColorSpace)),
            CancellationToken.None);

        Task.WaitAll(perceptualHashTask1, perceptualHashTask2);

        try
        {
            var distortion = perceptualHashTask1.Result.SumSquaredDistance(perceptualHashTask2.Result);
            return distortion;
        }
        catch (Exception)
        {
            // Temporary: lose this once the invalid colorspace issue is diagnosed
            return 1;
        }
    }

    private class PerceptualHashCache()
        : Cache<string, IPerceptualHash>(new SemaphoreSlimCache<string>());
}
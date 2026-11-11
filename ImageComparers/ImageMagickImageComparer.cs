using DuplicateFinder.Caches;
using ImageMagick;

namespace DuplicateFinder.ImageComparers;

internal abstract class ImageMagickImageComparer(
    double maxDistortion,
    ICache<string, MagickImage> cache,
    ErrorMetric comparisonErrorMetric,
    Channels comparisonChannels
) : IImageComparer
{
    private static bool _comparisonCacheIsDisposed;
    protected static readonly Cache<(string, string, ErrorMetric), bool> CachedComparisons = new ComparisonCache();

    public void Dispose()
    {
        cache.Dispose();
        try
        {
            if (!_comparisonCacheIsDisposed)
            {
                CachedComparisons.Dispose();
            }

            _comparisonCacheIsDisposed = true;
        }
        catch (ObjectDisposedException)
        {
            // Ignore, as this can happen if the cache is disposed before the comparer
        }
    }

    public async Task<bool> CompareAsync(string image1FullName, string image2FullName, CancellationToken cancellationToken)
    {
        if (image1FullName == image2FullName)
        {
            return true;
        }

        return await CachedComparisons.GetOrCreateAsync(
            (image1FullName, image2FullName, comparisonErrorMetric),
            async k => await CompareAsyncImpl(k.Item1, k.Item2, comparisonErrorMetric, cancellationToken),
            cancellationToken);
    }

    protected virtual MagickImage DecorateImage(MagickImage image)
    {
        var clone = (MagickImage)image.Clone();
        clone.ColorFuzz = new Percentage(5);
        return clone;
    }

    protected virtual List<int> GetRotationAngles(MagickImage image1, MagickImage image2)
    {
        List<int> rotationAngles = [];

        if (image1.Width == image2.Width && image1.Height == image2.Height)
        {
            // Image dimensions match, so compare as-is
            rotationAngles.Add(0);
        }
        else if (image1.Width == image2.Height && image1.Height == image2.Width)
        {
            // Image dimensions are swapped, so compare with 90 and -90 (aka 270) degree rotations
            rotationAngles.Add(90);
            rotationAngles.Add(270);
        }

        return rotationAngles;
    }


    private static async Task<MagickImage> CreateImageAsync(string imageFullName)
    {
        return await Task.FromResult(new MagickImage(imageFullName));
    }

    private async Task<bool> CompareAsyncImpl(string image1FullName, string image2FullName, ErrorMetric errorMetric, CancellationToken cancellationToken)
    {
        if (image1FullName == image2FullName)
        {
            return true;
        }

        try
        {
            var image1 = await cache.GetOrCreateAsync(
                image1FullName,
                CreateImageAsync,
                cancellationToken);
            image1 = DecorateImage(image1);

            var originalImage2 = await cache.GetOrCreateAsync(
                image2FullName,
                CreateImageAsync,
                cancellationToken);
            originalImage2 = DecorateImage(originalImage2);

            var rotationAngles = GetRotationAngles(image1, originalImage2);
            foreach (var rotationAngle in rotationAngles)
            {
                var image2 = (MagickImage)originalImage2.Clone();
                image2 = DecorateImage(image2);

                if (rotationAngle > 0)
                {
                    image2.Rotate(rotationAngle);
                }

                var distortion = CalculateDistortion(image1, image2, errorMetric);
                if (distortion < maxDistortion)
                {
                    return true;
                }
            }

            return false;
        }
        catch (MagickErrorException)
        {
            return false;
        }
    }

    protected virtual double CalculateDistortion(MagickImage image1, MagickImage image2, ErrorMetric errorMetric)
    {
        var distortion = image1.Compare(
            image2,
            errorMetric,
            comparisonChannels);
        return distortion;
    }

    private class ComparisonCache()
        : Cache<(string, string, ErrorMetric), bool>(new SemaphoreSlimCache<(string, string, ErrorMetric)>())
    {
        public override Task<bool> GetOrCreateAsync(
            (string, string, ErrorMetric) key,
            Func<(string, string, ErrorMetric), Task<bool>> objectFactory,
            CancellationToken cancellationToken)
        {
            var key1 = key.Item1;
            var key2 = key.Item2;
            if (key1.CompareTo(key2, StringComparison.OrdinalIgnoreCase) > 0)
            {
                (key1, key2) = (key2, key1);
            }

            return base.GetOrCreateAsync(
                (key1, key2, key.Item3),
                objectFactory,
                cancellationToken);
        }
    }
}
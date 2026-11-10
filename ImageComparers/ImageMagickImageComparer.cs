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
    private static readonly Cache<(string, string, ErrorMetric), bool> CachedComparisons = new ComparisonCache();

    public void Dispose()
    {
        cache.Dispose();
    }

    public async Task<bool> CompareAsync(string image1FullName, string image2FullName, CancellationToken cancellationToken)
    {
        if (image1FullName == image2FullName)
        {
            return true;
        }

        return await CachedComparisons.GetOrCreateAsync(
            (image1FullName, image2FullName, comparisonErrorMetric),
            async k => await CompareAsyncImpl(k.Item1, k.Item2, cancellationToken),
            cancellationToken);
    }

    protected virtual void DecorateImage(MagickImage image)
    {
        image.ColorFuzz = new Percentage(5);
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
            // Image dimensions are swapped, so compare with 90 and a further 180 degree (so 270 aka -90) degree rotations
            rotationAngles.Add(90);
            rotationAngles.Add(180);
        }

        return rotationAngles;
    }

    private async Task<bool> CompareAsyncImpl(string image1FullName, string image2FullName, CancellationToken cancellationToken)
    {
        if (image1FullName == image2FullName)
        {
            return true;
        }

        try
        {
            var image1 = await cache.GetOrCreateAsync(
                image1FullName,
                k => Task.FromResult(new MagickImage(k)),
                cancellationToken);
            DecorateImage(image1);

            var image2 = await cache.GetOrCreateAsync(
                image2FullName,
                k => Task.FromResult(new MagickImage(k)),
                cancellationToken);
            DecorateImage(image2);

            var rotationAngles = GetRotationAngles(image1, image2);
            foreach (var rotationAngle in rotationAngles)
            {
                if (rotationAngle > 0)
                {
                    image2.Rotate(rotationAngle);
                }

                var distortion = image1.Compare(
                    image2,
                    comparisonErrorMetric,
                    comparisonChannels);

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

    private class ComparisonCache()
        : Cache<(string, string, ErrorMetric), bool>(Semaphore)
    {
        private static readonly SemaphoreSlim Semaphore = new(1, 1);

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
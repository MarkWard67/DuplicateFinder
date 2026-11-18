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
        return await CachedComparisons.GetOrCreateAsync(
            (image1FullName, image2FullName, comparisonErrorMetric),
            k => CompareAsyncImpl(k.Item1, k.Item2, comparisonErrorMetric, cancellationToken),
            cancellationToken);
    }

    protected virtual MagickImage DecorateImage(MagickImage image)
    {
        image.ColorFuzz = new Percentage(10);
        return image;
    }

    private static MagickImage RotateImage(MagickImage image, int degrees)
    {
        var clone = (MagickImage)image.Clone();
        clone.Rotate(degrees);
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
        else
        {
            if (image1.Width > image1.Height)
            {
                if (image2.Width > image2.Height)
                {
                    // Both images are landscape, so compare as-is
                    rotationAngles.Add(0);
                }
                else
                {
                    // Image1 is landscape and Image2 is portrait, so compare with 90 and -90 (aka 270) degree rotations
                    rotationAngles.Add(90);
                    rotationAngles.Add(270);
                }
            }
        }
        
        return rotationAngles;
    }
    
    private static async Task<MagickImage> CreateImageAsync(string imageFullName)
    {
        var magickImage = new MagickImage(imageFullName);
        magickImage.ColorSpace = ColorSpace.Luv;
        return await Task.FromResult(magickImage);
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

            var rotationAngles = GetRotationAngles(image1, originalImage2).DefaultIfEmpty(0);
            return rotationAngles
                .Select(rotationAngle => rotationAngle == 0
                    ? originalImage2
                    : RotateImage(originalImage2, rotationAngle))
                .Select(image2 => CalculateDistortion(image1, image2, errorMetric))
                .Any(distortion => distortion < maxDistortion);
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
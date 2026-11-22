using DuplicateFinder.Caches;
using ImageMagick;

namespace DuplicateFinder.ImageComparers;

internal abstract class ImageMagickImageComparer(
    double maxDistortionSameFileName,
    double maxDistortionDifferentFileNames,
    ICache<string, MagickImage> cache,
    ErrorMetric comparisonErrorMetric,
    Channels comparisonChannels
) : IImageComparer
{
    private static bool _comparisonCacheIsDisposed;
    protected static readonly Cache<(string, string, ErrorMetric), ImageComparisonResult> CachedComparisons = new ComparisonCache();

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

    public async Task<ImageComparisonResult> CompareAsync(string image1FullName, string image2FullName, CancellationToken cancellationToken)
    {
        return await CachedComparisons.GetOrCreateAsync(
            (image1FullName, image2FullName, comparisonErrorMetric),
            k => CompareAsyncImpl(k.Item1, k.Item2, comparisonErrorMetric, cancellationToken),
            cancellationToken);
    }

    protected abstract void DecorateImage(MagickImage image);

    private static MagickImage RotateImage(MagickImage image, int degrees)
    {
        var clone = (MagickImage)image.Clone();
        clone.Rotate(degrees);
        return clone;
    }

    private static (MagickImage, MagickImage) Scale(MagickImage image1, MagickImage image2)
    {
        if (image1.Width == image2.Width && image1.Height == image2.Height)
        {
            return (image1, image2);
        }

        var area1 = image1.Width * image1.Height;
        var area2 = image2.Width * image2.Height;
        var scaleImage1 = area1 > area2;

        var scaledImage = (MagickImage)(scaleImage1 ? image1.Clone() : image2.Clone());
        var imageToCopy = scaleImage1 ? image2 : image1;
        scaledImage.Scale(new MagickGeometry(imageToCopy.Width, imageToCopy.Height) { FillArea = true, IgnoreAspectRatio = true });

        return scaleImage1 ? (scaledImage, imageToCopy) : (imageToCopy, scaledImage);
    }

    protected virtual List<int> GetRotationAngles(MagickImage image1, MagickImage image2)
    {
        List<int> rotationAngles = [];

        var isPortrait1 = image1.Height > image1.Width;
        var isPortrait2 = image2.Height > image2.Width;

        if (isPortrait1 == isPortrait2 || (image1.Width == image2.Width && image1.Height == image2.Height))
        {
            // Image dimensions match, so compare as-is
            rotationAngles.Add(0);
        }
        else if (isPortrait1 ^ isPortrait2 || (image1.Width == image2.Height && image1.Height == image2.Width))
        {
            // Image dimensions are swapped, so compare with 90 and -90 (aka 270) degree rotations
            rotationAngles.Add(90);
            rotationAngles.Add(270);
        }
        
        return rotationAngles;
    }
    
    private static async Task<MagickImage> CreateImageAsync(string imageFullName)
    {
        var magickImage = new MagickImage(imageFullName);
        magickImage.ColorSpace = ColorSpace.Luv;
        return await Task.FromResult(magickImage);
    }

    private async Task<ImageComparisonResult> CompareAsyncImpl(string image1FullName, string image2FullName, ErrorMetric errorMetric, CancellationToken cancellationToken)
    {
        if (image1FullName == image2FullName)
        {
            return ImageComparisonResult.Identical;
        }

        try
        {
            var baseName1 = Path.GetFileNameWithoutExtension(image1FullName);
            var baseName2 = Path.GetFileNameWithoutExtension(image2FullName);
            var sameFileName = baseName1.Length > 3 && baseName2.Length > 3 && (baseName1.Contains(baseName2, StringComparison.OrdinalIgnoreCase) || baseName2.Contains(baseName1, StringComparison.OrdinalIgnoreCase));
            var maxDistortion = sameFileName ? maxDistortionSameFileName : maxDistortionDifferentFileNames;

            var image1 = await cache.GetOrCreateAsync(
                image1FullName,
                CreateImageAsync,
                cancellationToken);

            var originalImage2 = await cache.GetOrCreateAsync(
                image2FullName,
                CreateImageAsync,
                cancellationToken);

            var rotationAngles = GetRotationAngles(image1, originalImage2).DefaultIfEmpty(0);
            var match = rotationAngles
                .Select(rotationAngle => rotationAngle == 0
                    ? originalImage2
                    : RotateImage(originalImage2, rotationAngle))
                .Select(image2 =>
                {
                    var (scaledImage1, scaledImage2) = Scale(image1, image2);
                    DecorateImage(scaledImage1);

                    var distortion = CalculateDistortion(scaledImage1, scaledImage2, errorMetric);

                    return new ImageMagickImageComparisonResult
                    {
                        Type = distortion <= maxDistortion ? ImageComparisonResultType.PartialMatch : ImageComparisonResultType.NoMatch,
                        Channels = comparisonChannels,
                        ErrorMetric = comparisonErrorMetric,
                        Distortion = distortion
                    };
                })
                .FirstOrDefault(comparison => comparison.Type.ImagesMatch());

            return match ?? new ImageComparisonResult { Type = ImageComparisonResultType.NoMatch };
        }
        catch (MagickErrorException ex)
        {
            return ImageComparisonResult.Failure(ex);
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
        : Cache<(string, string, ErrorMetric), ImageComparisonResult>(new SemaphoreSlimCache<(string, string, ErrorMetric)>())
    {
        public override Task<ImageComparisonResult> GetOrCreateAsync(
            (string, string, ErrorMetric) key,
            Func<(string, string, ErrorMetric), Task<ImageComparisonResult>> objectFactory,
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
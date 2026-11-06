using System.Collections.Concurrent;
using ImageMagick;

namespace DuplicateFinder;

internal class ImageMagickImageComparer(double maxDistortion) : IImageComparer
{
    private readonly ConcurrentDictionary<string, MagickImage> _imageCache = new(67, [], StringComparer.OrdinalIgnoreCase);

    public void Dispose()
    {
        DisposeCachedImages();
    }

    private void DisposeCachedImages()
    {
        var imagesToDispose = _imageCache.Values.ToArray();
        _imageCache.Clear();

        foreach (var image in imagesToDispose)
        {
            image.Dispose();
        }
    }

    public bool Compare(string image1FullName, string image2FullName)
    {
        if (image1FullName == image2FullName)
        {
            return true;
        }

        MagickImage? image1 = null;
        MagickImage? image2 = null;

        try
        {
            image1 = GetOrCreateImage(image1FullName);
            image1.ColorFuzz = new Percentage(5);

            image2 = GetOrCreateImage(image2FullName);
            image2.ColorFuzz = new Percentage(5);

            const Channels comparisonChannels = Channels.Red | Channels.Green | Channels.Blue | Channels.Alpha;
            const ErrorMetric comparisonErrorMetric = ErrorMetric.MeanAbsolute;

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

            foreach (var rotationAngle in rotationAngles)
            {
                if (rotationAngle > 0)
                {
                    image2.Rotate(rotationAngle);
                }

                // Compare using Absolute Error Metric (matches pixel differences)
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
        finally
        {
            CacheImages(image1, image2);
        }
    }

    public void Clear()
    {
        ClearIf(0);
    }

    public void ClearIf(int maximumItems)
    {
        if (_imageCache.Count <= maximumItems)
        {
            return;
        }

        DisposeCachedImages();
        Clear();
    }

    private MagickImage GetOrCreateImage(string imageFullName)
    {
        // If there is an image cached under this key, remove it from the cache and return it. Otherwise, create a new MagickImage instance.
        return _imageCache.Remove(imageFullName, out var image) ? image : new MagickImage(imageFullName);
    }

    private void CacheImages(params IEnumerable<MagickImage?> images)
    {
        foreach (var image in images)
        {
            if (image is null)
            {
                continue;
            }

            _imageCache.TryAdd(image.FileName!, image);
        }
    }

}
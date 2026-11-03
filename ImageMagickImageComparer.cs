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
        foreach (var kvp in _imageCache) kvp.Value.Dispose();
    }

    public bool Compare(string image1FullName, string image2FullName)
    {
        if (image1FullName == image2FullName)
        {
            return true;
        }

        try
        {
            //////////var extension1 = Path.GetExtension(image1FullName);
            //////////var extension2 = Path.GetExtension(image2FullName);
            //////////if (extension1 == "" || extension2 == "" ||
            //////////    !extension1.Equals(extension2, StringComparison.OrdinalIgnoreCase))
            //////////{
            //////////    return false;
            //////////}

            var image1 = GetOrCreateImage(image1FullName);
            image1.ColorFuzz = new Percentage(5);

            var image2 = GetOrCreateImage(image2FullName);
            image2.ColorFuzz = new Percentage(5);

            const Channels comparisonChannels = Channels.Red | Channels.Green | Channels.Blue | Channels.Alpha;
            const ErrorMetric comparisonErrorMetric = ErrorMetric.Absolute;

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
    }

    public void Clear()
    {
        _imageCache.Clear();
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
        return _imageCache.GetOrAdd(imageFullName, path => new MagickImage(path));
    }
}
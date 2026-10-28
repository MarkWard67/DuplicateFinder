using System.Collections.Concurrent;
using DuplicateFinder.Files;
using ImageMagick;

namespace DuplicateFinder;

internal class ImageComparer(IFileHasher fileHasher) : IDisposable
{
    private readonly ConcurrentDictionary<string, MagickImage> _imageCache = new();

    public void Dispose()
    {
        foreach (var kvp in _imageCache) kvp.Value.Dispose();
    }

    public bool Compare(string image1FullName, string image2FullName)
    {
        var hash1 = fileHasher.ComputeFileHash(image1FullName);
        var hash2 = fileHasher.ComputeFileHash(image2FullName);
        if (hash1.Equals(hash2))
        {
            return true;
        }

        try
        {
            var extension1 = Path.GetExtension(image1FullName);
            var extension2 = Path.GetExtension(image2FullName);
            if (!extension1.Equals(extension2, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var image1 = GetOrCreateImage(image1FullName);
            image1.ColorFuzz = new Percentage(5);

            var image2 = GetOrCreateImage(image2FullName);
            image2.ColorFuzz = new Percentage(5);

            if (image1.Width != image2.Width || image1.Height != image2.Height)
            {
                return false;
            }

            // Compare using Absolute Error Metric (matches pixel differences)
            var distortion = image1.Compare(
                image2,
                ErrorMetric.Absolute,
                Channels.Red | Channels.Green | Channels.Blue | Channels.Alpha);

            return distortion < 1e-9;
        }
        catch (MagickErrorException)
        {
            return false;
        }
    }

    private MagickImage GetOrCreateImage(string imageFullName)
    {
        return _imageCache.GetOrAdd(imageFullName, path => new MagickImage(path));
    }
}
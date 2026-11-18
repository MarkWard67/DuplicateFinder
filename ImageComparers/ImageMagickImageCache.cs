using DuplicateFinder.Caches;
using ImageMagick;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace DuplicateFinder.ImageComparers;

internal class ImageMagickImageCache : Cache<string, MagickImage>
{
    private ImageMagickImageCache()
        : base(
            new SemaphoreSlimCache<string>())
    {
    }

    public static ImageMagickImageCache Instance { get; } = new();

    protected override MemoryCacheEntryOptions CacheEntryOptionsFactory(CancellationToken cancellationToken)
    {
        return new MemoryCacheEntryOptions()
            .AddExpirationToken(new CancellationChangeToken(cancellationToken))
            .SetSlidingExpiration(TimeSpan.FromMinutes(10))
            .RegisterPostEvictionCallback(AfterEviction, null);
    }

    // Define the callback function (must be static or a self-contained lambda to prevent memory leaks)
    protected virtual void AfterEviction(
        object key,
        object? value,
        EvictionReason reason,
        object? state)
    {
        if (value is IDisposable disposableValue)
        {
            disposableValue.Dispose();
        }
    }
}
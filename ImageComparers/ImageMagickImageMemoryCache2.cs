using DuplicateFinder.Caches;
using ImageMagick;
using Microsoft.Extensions.Caching.Memory;

namespace DuplicateFinder.ImageComparers;

internal class ImageMagickImageMemoryCache2 : Cache<string, MagickImage>
{
    // Holds a unique semaphore lock for each active cache key
    private static readonly SemaphoreSlim Lock = new(1, 1);

    private ImageMagickImageMemoryCache2()
        : base(Lock)
    {
    }

    public static ImageMagickImageMemoryCache2 Instance { get; } = new();

    protected override MemoryCacheEntryOptions CacheEntryOptionsFactory()
    {
        return new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(1))
            .RegisterPostEvictionCallback(AfterEviction, null);
    }

    // Define the callback function (must be static or a self-contained lambda to prevent memory leaks)
    protected virtual void AfterEviction(object key, object? value, EvictionReason reason, object? state)
    {
        switch (reason)
        {
            case EvictionReason.TokenExpired:
            case EvictionReason.Expired:
            case EvictionReason.Removed:
            case EvictionReason.Replaced:
                if (value is IDisposable disposableValue)
                {
                    disposableValue.Dispose();
                }

                break;

            case EvictionReason.Capacity:
            case EvictionReason.None:
            default:
                break;
        }
    }
}
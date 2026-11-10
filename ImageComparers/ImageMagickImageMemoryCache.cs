//////////using ImageMagick;
//////////using Microsoft.Extensions.Caching.Memory;

//////////namespace DuplicateFinder.ImageComparers;

//////////internal class ImageMagickImageMemoryCache : IImageMagickImageMemoryCache
//////////{
//////////    private readonly Microsoft.Extensions.Caching.Memory.MemoryCache _cache = new(new MemoryCacheOptions());

//////////    // Holds a unique semaphore lock for each active cache key
//////////    private static readonly SemaphoreSlim Lock = new(1, 1);

//////////    private ImageMagickImageMemoryCache()
//////////    {
//////////    }

//////////    public static ImageMagickImageMemoryCache Instance { get; } = new();

//////////    public void Dispose()
//////////    {
//////////        _cache.Dispose();
//////////    }

//////////    public async Task<MagickImage> GetOrCreateAsync(
//////////        string fileFullName,
//////////        CancellationToken cancellationToken)
//////////    {
//////////        // Fast Path: If it's a cache hit, return immediately without locking
//////////        if (_cache.TryGetValue(fileFullName, out MagickImage? cachedObject) && cachedObject != null)
//////////        {
//////////            return cachedObject;
//////////        }

//////////        // Get or create a lock specific only to this key
//////////        await Lock.WaitAsync(cancellationToken);

//////////        try
//////////        {
//////////            // Double-Check: Another thread might have populated the cache 
//////////            // while this thread was waiting for the semaphore.
//////////            if (_cache.TryGetValue(fileFullName, out cachedObject) && cachedObject != null)
//////////            {
//////////                return cachedObject;
//////////            }

//////////            cachedObject = new MagickImage(fileFullName);
//////////            _cache.Set(
//////////                fileFullName,
//////////                cachedObject,
//////////                new MemoryCacheEntryOptions()
//////////                    .SetSlidingExpiration(TimeSpan.FromMinutes(10))
//////////                    .RegisterPostEvictionCallback(AfterEviction, null)
//////////            );

//////////            return cachedObject;
//////////        }
//////////        finally
//////////        {
//////////            Lock.Release();
//////////        }
//////////    }

//////////    // Define the callback function (must be static or a self-contained lambda to prevent memory leaks)
//////////    private static void AfterEviction(object key, object? value, EvictionReason reason, object? state)
//////////    {
//////////        switch (reason)
//////////        {
//////////            case EvictionReason.TokenExpired:
//////////            case EvictionReason.Expired:
//////////            case EvictionReason.Removed:
//////////            case EvictionReason.Replaced:
//////////                if (value is IDisposable disposableValue)
//////////                {
//////////                    disposableValue.Dispose();
//////////                }

//////////                break;

//////////            case EvictionReason.Capacity:
//////////            case EvictionReason.None:
//////////            default:
//////////                break;
//////////        }
//////////    }
//////////}
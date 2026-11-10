using Microsoft.Extensions.Caching.Memory;

namespace DuplicateFinder.Caches;

internal abstract class Cache<TKey, TValue>(SemaphoreSlim semaphore) : ICache<TKey, TValue>
{
    private readonly MemoryCache _cache = new(new MemoryCacheOptions());

    public void Dispose()
    {
        _cache.Dispose();
    }

    public virtual async Task<TValue> GetOrCreateAsync(
        TKey key,
        Func<TKey, Task<TValue>> objectFactoryAsync,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(key);

        // Fast Path: If it's a cache hit, return immediately without locking
        if (_cache.TryGetValue(key, out TValue? cachedObject) && cachedObject != null)
        {
            return cachedObject;
        }

        // Get or create a lock specific only to this key
        await WaitLock(cancellationToken);

        try
        {
            // Double-Check: Another thread might have populated the cache 
            // while this thread was waiting for the semaphore.
            if (_cache.TryGetValue(key, out cachedObject) && cachedObject != null)
            {
                return cachedObject;
            }

            cachedObject = await objectFactoryAsync(key);
            _cache.Set(
                key,
                cachedObject,
                CacheEntryOptionsFactory()
            );

            return cachedObject!;
        }
        finally
        {
            semaphore.Release();
        }
    }

    protected virtual MemoryCacheEntryOptions CacheEntryOptionsFactory()
    {
        return new MemoryCacheEntryOptions();
    }

    protected virtual async Task WaitLock(CancellationToken cancellationToken)
    {
        await semaphore.WaitAsync(TimeSpan.FromMinutes(1), cancellationToken);
    }
}
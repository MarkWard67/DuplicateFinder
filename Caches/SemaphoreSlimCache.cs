using Microsoft.Extensions.Caching.Memory;

namespace DuplicateFinder.Caches;

internal abstract class SemaphoreSlimCache
{
    protected static readonly SemaphoreSlim Semaphore = new(1, 1);
}

internal class SemaphoreSlimCache<TKey> : SemaphoreSlimCache, ISemaphoreCache<TKey>
{
    private readonly MemoryCache _cache = new(new MemoryCacheOptions());

    public virtual async Task<SemaphoreSlim> GetOrCreateAsync(
        TKey key,
        Func<TKey, Task<SemaphoreSlim>> semaphoreFactoryAsync,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(key);

        // Fast Path: If it's a cache hit, return immediately without locking
        if (_cache.TryGetValue(key, out SemaphoreSlim? cachedObject) && cachedObject != null)
        {
            return cachedObject;
        }

        // Get or create a lock specific only to this key
        await Semaphore.WaitAsync(cancellationToken);

        try
        {
            // Double-Check: Another thread might have populated the cache 
            // while this thread was waiting for the semaphore.
            if (_cache.TryGetValue(key, out cachedObject) && cachedObject != null)
            {
                return cachedObject;
            }

            cachedObject = await semaphoreFactoryAsync(key);
            _cache.Set(
                key,
                cachedObject,
                new MemoryCacheEntryOptions()
            );

            return cachedObject;
        }
        finally
        {
            Semaphore.Release();
        }
    }

    public virtual Task<SemaphoreSlim> CreateSemaphore()
    {
        return Task.FromResult(new SemaphoreSlim(1, 1));
    }

    public void Dispose()
    {
        _cache.Dispose();
    }
}
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace DuplicateFinder.Caches;

internal abstract class Cache<TKey, TValue>(ISemaphoreCache<TKey> semaphoreCache) : ICache<TKey, TValue>
{
    private readonly MemoryCache _cache = new(new MemoryCacheOptions());

    //////////protected Cache(SemaphoreSlim semaphore)
    //////////{
    //////////    _semaphoreCache  = new DegenerateSemaphoreCache(semaphore);
    //////////}

    public void Dispose()
    {
        semaphoreCache.Dispose();
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
        var semaphore = await GetSemaphore(key, cancellationToken);
        await semaphore.WaitAsync(cancellationToken);

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
                CacheEntryOptionsFactory(cancellationToken)
            );

            return cachedObject;
        }
        finally
        {
            semaphore.Release();
        }
    }

    private async Task<SemaphoreSlim> GetSemaphore(
        [DisallowNull] TKey key,
        CancellationToken cancellationToken)
    {
        var semaphore = await semaphoreCache.GetOrCreateAsync(
            key,
            _ => semaphoreCache.CreateSemaphore(),
        cancellationToken);
        return semaphore;
    }

    protected virtual MemoryCacheEntryOptions CacheEntryOptionsFactory(CancellationToken cancellationToken)
    {
        return new MemoryCacheEntryOptions()
            .AddExpirationToken(
                new CancellationChangeToken(cancellationToken));
    }
}
namespace DuplicateFinder.Caches;

internal record SemaphoreSlimCacheKey(
    string UltimateKey,
    Guid CacheGuid);
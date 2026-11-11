namespace DuplicateFinder.Caches;

internal interface ISemaphoreCache<TKey> : ICache<TKey, SemaphoreSlim>
{
    Task<SemaphoreSlim> CreateSemaphore();
}
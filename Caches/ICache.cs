namespace DuplicateFinder.Caches;

internal interface ICache<TKey, TValue> : IDisposable
{
    public Task<TValue> GetOrCreateAsync(
        TKey key,
        Func<TKey, Task<TValue>> objectFactoryAsync,
        CancellationToken cancellationToken);

}
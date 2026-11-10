namespace DuplicateFinder.ImageComparers;

internal class AggregateImageComparer(params IEnumerable<IImageComparer> comparers) : IImageComparer
{
    public void Dispose()
    {
        foreach (var comparer in comparers)
        {
            comparer.Dispose();
        }
    }

    public async Task<bool> CompareAsync(string image1FullName, string image2FullName, CancellationToken cancellationToken)
    {
        var tasks = comparers.Select(comparer => comparer.CompareAsync(image1FullName, image2FullName, cancellationToken));
        var comparisons = await Task.WhenAll(tasks);
        return comparisons.Any(x => true);
    }
}
namespace DuplicateFinder.ImageComparers;

internal record ImageComparisonResult
{
    public required ImageComparisonResultType Type
    {
        get;
        init;
    }

    public Exception? Exception { get; init; }

    public static ImageComparisonResult Cancellation => new()
    {
        Type = ImageComparisonResultType.Cancellation
    };

    public static ImageComparisonResult Identical => new()
    {
        Type = ImageComparisonResultType.FullMatch
    };

    public static ImageComparisonResult Failure(Exception ex)
    {
        return new ImageComparisonResult
        {
            Type = ImageComparisonResultType.Failure,
            Exception = ex
        };
    }
}
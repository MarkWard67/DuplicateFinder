namespace DuplicateFinder.ImageComparers;

internal enum ImageComparisonResultType
{
    Failure,

    Cancellation,

    NoMatch,

    PartialMatch,
    
    FullMatch
}

internal static class ComparisonResultTypeExtensions
{
    public static bool ImagesMatch(this ImageComparisonResultType resultType)
    {
        return resultType is ImageComparisonResultType.PartialMatch or ImageComparisonResultType.FullMatch;
    }
}
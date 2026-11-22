using ImageMagick;

namespace DuplicateFinder.ImageComparers;

internal record ImageMagickImageComparisonResult : ImageComparisonResult
{
    public ErrorMetric ErrorMetric { get; init; }
    public Channels Channels { get; init; }
    public double Distortion { get; init; }
}
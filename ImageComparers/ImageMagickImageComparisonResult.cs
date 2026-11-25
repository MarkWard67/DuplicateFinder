using DuplicateFinder.Reporters;
using ImageMagick;

namespace DuplicateFinder.ImageComparers;

internal record ImageMagickImageComparisonResult : ImageComparisonResult
{
    public required ErrorMetric ErrorMetric { get; init; }
    public required Channels Channels { get; init; }
    public required double Distortion { get; init; }

    public required Percentage ColorFuzz { get; init; }

    public override string ToString()
    {
        return ToCsv();
    }

    public override string ToCsv(char separator = CsvValues.Separator, char delimiter = CsvValues.Delimiter)
    {
        var values = new List<object>
        {
            ColorFuzz,
            $"{Distortion:0.0000}",
            ErrorMetric,
            Channels
        };
        return $"{CsvReporter.ToCsv(values)}{separator}{base.ToCsv(separator, delimiter)}";
    }
}
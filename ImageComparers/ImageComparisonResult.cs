using System.ComponentModel.DataAnnotations;
using DuplicateFinder.Reporters;

namespace DuplicateFinder.ImageComparers;

internal record ImageComparisonResult : ICsvReportable
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

    public override string ToString()
    {
        return ToCsv();
    }

    public virtual string ToCsv(char separator = CsvValues.Separator, char delimiter = CsvValues.Delimiter)
    {
        var values = new List<object> { Type, Exception?.Message ?? string.Empty };
        return CsvReporter.ToCsv(values, separator, delimiter);
    }
}
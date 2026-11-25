namespace DuplicateFinder.Reporters;

internal static class CsvReporter
{
    public static string ToCsv(
        IEnumerable<object?> values,
        char separator = CsvValues.Separator,
        char? delimiter = null)
    {
        var delimiterString = delimiter == null ? string.Empty : delimiter.ToString();
        var reportableValues = values.Select(x =>
        {
            if (x is ICsvReportable csvReportable)
            {
                return csvReportable.ToCsv(separator, delimiter ?? CsvValues.Delimiter);
            }

            return $"{delimiterString}{x?.ToString() ?? string.Empty}{delimiterString}";
        });
        return string.Join(separator, reportableValues);
    }
}
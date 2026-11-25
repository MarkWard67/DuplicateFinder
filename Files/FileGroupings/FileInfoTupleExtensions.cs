using System.Text;
using DuplicateFinder.Reporters;

namespace DuplicateFinder.Files.FileGroupings;

internal static class FileInfoTupleExtensions
{
    public static string ToCsvImpl(
        this IFileInfoTuple fileInfoTuple,
        char separator = CsvValues.Separator,
        char delimiter = CsvValues.Delimiter,
        Dictionary<string, string>? verbs = null)
    {
        var values = (
                from t in fileInfoTuple
                let verb = verbs?.GetValueOrDefault(t.FullName)
                select $"{verb ?? string.Empty}{delimiter}{t.FullName}{delimiter}")
            .Cast<object>()
            .ToList();

        var propertiesValue = fileInfoTuple.Properties?.ToCsv(separator, delimiter);
        if (propertiesValue != null)
        {
            values.Add(propertiesValue);
        }

        return CsvReporter.ToCsv(values, separator);
    }
}
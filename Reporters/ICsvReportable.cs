namespace DuplicateFinder.Reporters;

internal interface ICsvReportable
{
    public string ToCsv(char separator = CsvValues.Separator, char delimiter = CsvValues.Delimiter);
}
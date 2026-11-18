using System.Globalization;
using System.Text.RegularExpressions;
using DuplicateFinder.Caches;

namespace DuplicateFinder.Files.FilePairers;

internal partial class DateTimeTakenFilePairer : FilePairer
{
    private static readonly DateTimeTakenCache DateTimeCache = new();

    private readonly Regex[] _dateTimeTakenRegexes =
    [
        DateTimeRegex1(),
        DateTimeRegex2()
    ];

    private readonly Func<int, int, int, TimeOnly> _timeOnlyFactory;

    public DateTimeTakenFilePairer(Func<int, int, int, TimeOnly> timeOnlyFactory)
    {
        _timeOnlyFactory = timeOnlyFactory;
    }

    public DateTimeTakenFilePairer(
        string redundantFilesFolderPath,
        Func<int, int, int, TimeOnly> timeOnlyFactory)
        : base(redundantFilesFolderPath)
    {
        _timeOnlyFactory = timeOnlyFactory;
    }

    protected override bool ShouldPair(FileInfo fileInfo1, FileInfo fileInfo2)
    {
        var dateTimeTaken1 = DateTimeCache.GetOrCreateAsync(
                fileInfo1.FullName,
                _ => Task.Run(() => GetDateTimeTaken(fileInfo1)),
                CancellationToken.None)
            .GetAwaiter()
            .GetResult();

        var dateTimeTaken2 = DateTimeCache.GetOrCreateAsync(
                fileInfo2.FullName,
                _ => Task.Run(() => GetDateTimeTaken(fileInfo2)),
                CancellationToken.None)
            .GetAwaiter()
            .GetResult();

        if (dateTimeTaken1 == null || dateTimeTaken2 == null)
        {
            return false;
        }

        dateTimeTaken1 = GetDateTimeTaken(dateTimeTaken1);
        dateTimeTaken2 = GetDateTimeTaken(dateTimeTaken2);

        return dateTimeTaken1.HasValue && dateTimeTaken2.HasValue && dateTimeTaken1.Value == dateTimeTaken2.Value;
    }

    [GeneratedRegex(
        @"(?<year>\d{4})-(?<month>\d{2})-(?<day>\d{2})(\s+(?<hour>\d{2})-(?<minute>\d{2})(-(?<second>\d{2}))?)?")]
    private static partial Regex DateTimeRegex1();

    [GeneratedRegex(
        @"(?<year>\d{4})(?<month>\d{2})(?<day>\d{2})(_(?<hour>\d{2})(?<minute>\d{2})((?<second>\d{2}))?)?")]
    private static partial Regex DateTimeRegex2();

    private DateTime? GetDateTimeTaken(FileInfo fileInfo)
    {
        var dateTimeRegex = _dateTimeTakenRegexes.FirstOrDefault(x => x.IsMatch(fileInfo.Name));

        var match = dateTimeRegex?.Matches(fileInfo.Name).FirstOrDefault();
        if (match == null)
        {
            return null;
        }

        var year = int.Parse(match.Groups["year"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture);
        var month = int.Parse(match.Groups["month"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture);
        var day = int.Parse(match.Groups["day"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture);
        var hour = match.Groups["hour"].Success
            ? int.Parse(match.Groups["hour"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture)
            : 0;
        var minute = match.Groups["minute"].Success
            ? int.Parse(match.Groups["minute"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture)
            : 0;
        var second = match.Groups["second"].Success
            ? int.Parse(match.Groups["second"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture)
            : 0;

        if (year < 1966 || year > DateTime.Now.Year || month is < 1 or > 12 || day is < 1 or > 31)
        {
            return null;
        }

        try
        {
            return GetDateTimeTaken(new DateOnly(year, month, day), new TimeOnly(hour, minute, second));
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    private static DateTime? GetDateTimeTaken(DateOnly dateOnly, TimeOnly timeOnly)
    {
        return new DateTime(
            dateOnly,
            timeOnly,
            DateTimeKind.Utc);
    }

    private DateTime? GetDateTimeTaken(DateTime? fullDateTime)
    {
        if (!fullDateTime.HasValue)
        {
            return null;
        }

        var dateOnly = new DateOnly(
            fullDateTime.Value.Year,
            fullDateTime.Value.Month,
            fullDateTime.Value.Day
        );

        var timeOnly = _timeOnlyFactory(
            fullDateTime.Value.Hour,
            fullDateTime.Value.Minute,
            fullDateTime.Value.Second
        );

        return GetDateTimeTaken(
            dateOnly,
            timeOnly);
    }

    private class DateTimeTakenCache() : Cache<string, DateTime?>(new SemaphoreSlimCache<string>());
}
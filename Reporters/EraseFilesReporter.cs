using System.Text;
using System.Text.RegularExpressions;
using DuplicateFinder.Files.FileGroupings;

namespace DuplicateFinder.Reporters;

internal class EraseFilesReporter : DefaultReporter
{
    private readonly HashSet<string> _eraseFromFolderPaths = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<Regex> _eraseFromFolderRegexes = [];

    public EraseFilesReporter(params IEnumerable<object> eraseFromFolderSpecs)
    {
        foreach (var folderSpec in eraseFromFolderSpecs)
        {
            switch (folderSpec)
            {
                case string folderPath:
                    _eraseFromFolderPaths.Add(folderPath);
                    break;

                case Regex folderRegex:
                    _eraseFromFolderRegexes.Add(folderRegex);
                    break;

                case null:
                    break;

                default:
                    throw new ArgumentException("Strings or Regexes only, please", nameof(eraseFromFolderSpecs));
            }
        }
    }

    public override string Report(IEnumerable<IFileInfoGrouping> fileGroupings)
    {
        var sb = new StringBuilder();
        foreach (var fileGrouping in fileGroupings)
        {
            var filesNamesInGroupingByDeletability = fileGrouping
                .SelectMany(x => Enumerable.Range(0, x.Length).Select(y => x[y]))
                .GroupBy(x => IsAnEraseFromDirectory(x.DirectoryName))
                .ToDictionary(
                    x => x.Key,
                    x => x.Select(y => y.FullName).ToHashSet());

            var filesNamesToDelete = filesNamesInGroupingByDeletability.GetValueOrDefault(true) ?? [];

            if (filesNamesToDelete.Count == 0)
            {
                return base.Report([fileGrouping]);
            }

            var filesNamesToKeep = filesNamesInGroupingByDeletability.GetValueOrDefault(false) ?? [];

            if (filesNamesToDelete.Count > 0 && filesNamesToKeep.Count == 0)
            {
                var lastFileToDelete = filesNamesToDelete.Last();
                filesNamesToDelete.Remove(lastFileToDelete);
                filesNamesToKeep.Add(lastFileToDelete);
            }

            var verbs = new Dictionary<string, string>();
            foreach (var fileInfoTuple in fileGrouping)
            {
                foreach (var fileInGroup in fileInfoTuple)
                {
                    verbs.Add(
                        fileInGroup.FullName,
                        filesNamesToDelete.Contains(fileInGroup.FullName) ? "@ERASE " : "@REM ");
                }
            }

            var values = new List<object> { $"{CsvValues.Delimiter}{fileGrouping.Type}{CsvValues.Delimiter}" };
            values.AddRange(fileGrouping
                .Select(fileInfoTuple => fileInfoTuple.ToCsvImpl(CsvValues.Separator, CsvValues.Delimiter, verbs)));
            sb.Append(CsvReporter.ToCsv(values));
        }

        return sb.ToString();
    }

    private bool IsAnEraseFromDirectory(string? directoryPath)
    {
        return directoryPath != null && (_eraseFromFolderPaths.Contains(directoryPath) || _eraseFromFolderRegexes.Any(x => x.IsMatch(directoryPath)));
    }
}
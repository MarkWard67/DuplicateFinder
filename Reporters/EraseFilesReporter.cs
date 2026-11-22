using System.Text;
using DuplicateFinder.Files.FileGroupings;

namespace DuplicateFinder.Reporters;

internal class EraseFilesReporter : DefaultReporter
{
    private readonly HashSet<string> _eraseFromFolderPaths = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> _eraseFromRootFolderPaths = new(StringComparer.OrdinalIgnoreCase);

    public EraseFilesReporter(params IEnumerable<string> eraseFromFolderPaths)
    {
        var folderPaths = eraseFromFolderPaths
            .Distinct()
            .ToArray();

        foreach (var folderPath in folderPaths)
        {
            if (folderPath.EndsWith(@"\*"))
            {
                var rootFolderPath = folderPath[..^1];
                _eraseFromRootFolderPaths.Add(rootFolderPath);
            }
            else
            {
                _eraseFromFolderPaths.Add(folderPath);
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

            foreach (var fileInfoTuple in fileGrouping)
            {
                if (fileInfoTuple.Properties != null)
                {
                    sb.AppendLine($"@REM {fileInfoTuple.Properties}");
                }

                for (var i = 0; i < fileInfoTuple.Length; i++)
                {
                    var fileInGroup = fileInfoTuple[i];
                    if (filesNamesToDelete.Contains(fileInGroup.FullName))
                    {
                        sb.AppendLine($"@ERASE \"{fileInGroup.FullName}\"");
                    }
                    else
                    {
                        sb.AppendLine($"@REM {fileInGroup.FullName}");
                    }
                }
            }
        }

        return sb.ToString();
    }

    private bool IsAnEraseFromDirectory(string? directoryPath)
    {
        return directoryPath != null && (_eraseFromFolderPaths.Contains(directoryPath) || _eraseFromRootFolderPaths.Any(directoryPath.StartsWith));
    }
}
using System.Text;
using DuplicateFinder.Files.FileGroupings;

namespace DuplicateFinder.Reporters;

internal class EraseFilesReporter : DefaultReporter
{
    private readonly HashSet<string> _eraseFromFolderPaths = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> _eraseFromRootFolderPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

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
            var filesInGroupingByDeletability = fileGrouping
                .GroupBy(x => IsAnEraseFromDirectory(x.DirectoryName))
                .ToDictionary(
                    x => x.Key,
                    x => x.ToList());

            var filesToDelete = filesInGroupingByDeletability.GetValueOrDefault(true) ?? [];

            if (filesToDelete.Count == 0)
            {
                return base.Report([fileGrouping]);
            }

            var filesToKeep = filesInGroupingByDeletability.GetValueOrDefault(false) ?? [];

            if (filesToDelete.Count > 0 && filesToKeep.Count == 0)
            {
                var lastFileToDelete = filesToDelete.Last();
                filesToDelete = filesToDelete.Take(filesToDelete.Count - 1).ToList();
                filesToKeep.Add(lastFileToDelete);
            }

            foreach (var fileInGroup in filesToKeep)
            {
                sb.AppendLine($"@REM {fileInGroup.FullName}");
            }

            foreach (var fileInGroup in filesToDelete)
            {
                sb.AppendLine($"ERASE \"{fileInGroup.FullName}\"");
            }
        }

        return sb.ToString();
    }

    private bool IsAnEraseFromDirectory(string? directoryPath)
    {
        return directoryPath != null && (_eraseFromFolderPaths.Contains(directoryPath) || _eraseFromRootFolderPaths.Any(directoryPath.StartsWith));
    }
}
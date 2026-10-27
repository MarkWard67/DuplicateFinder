using DuplicateFinder.Files;
using DuplicateFinder.Files.FileFinders;

namespace DuplicateFinder.FileComparers;

internal class WithinFolderSimilarFilesFinder(
    IFileFinder fileFinder,
    IFileHasher fileHasher)
{
    public (FileInfoWrapper, FileInfoWrapper[])[] FindSimilarFiles()
    {
        var allFileInfos = fileFinder.FindFiles();

        var duplicatesFinder = new WithinFolderDuplicateFileFinder(fileFinder, fileHasher);
        var duplicates = duplicatesFinder.FindDuplications();

        var allDuplicateFileFullNames = duplicates
            .SelectMany(x => x)
            .Select(x => x.FileInfo.FullName)
            .Distinct()
            .ToHashSet();

        var fileInfoGroupings = allFileInfos
            .Where(x => !allDuplicateFileFullNames.Contains(x.FileInfo.FullName))
            .GroupBy(x => x.FileInfo.DirectoryName)
            .Select(g => g.ToArray())
            .Where(x => x.Length > 1)
            .SelectMany(x => x
                .GroupBy(x => (x.FileInfo.Extension.ToLowerInvariant(), x.FileInfo.Length))
                .Select(g => g.ToArray())
                .Where(x => x.Length > 1)
                .ToArray())
            .ToArray();

        var result = new List<(FileInfoWrapper, FileInfoWrapper[])>();
        foreach (var fileInfoGrouping in fileInfoGroupings)
        {
            //////////var directoryFileInfosGroupings = directoryFileInfos
            //////////    .GroupBy(x => (x.FileInfo.Extension.ToLowerInvariant(), x.FileInfo.Length))
            //////////    .Select(g => g.ToArray())
            //////////    .Where(x => x.Length > 1)
            //////////    .ToArray();

            //////////foreach (var fileInfosGrouping in directoryFileInfosGroupings)
            //////////{
                using var imageComparer = new ImageComparer();

                for (var i = 0; i < fileInfoGrouping.Length - 1; i++)
                {
                    var similarToi = new List<FileInfoWrapper>();
                    for (var j = i + 1; j < fileInfoGrouping.Length; j++)
                    {

                        var isMatch = imageComparer.Compare(
                            fileInfoGrouping[i].FileInfo.FullName,
                            fileInfoGrouping[j].FileInfo.FullName);
                        if (!isMatch)
                        {
                            continue;
                        }

                        similarToi.Add(fileInfoGrouping[j]);
                        break;
                    }

                    if (similarToi.Count > 0)
                    {
                        result.Add((fileInfoGrouping[i], similarToi.OrderByDescending(x => x.FileInfo.LastAccessTimeUtc).ToArray()));
                    }
                }
            ////////////}
        }

        return result.ToArray();
    }
}
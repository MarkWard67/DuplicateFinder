using System.Text.RegularExpressions;
using DuplicateFinder.Files.FileGroupings;

namespace DuplicateFinder.Files.FileGroupers;

internal partial class SimilarImagesFileGrouper(
    IFileHasher fileHasher,
    IImageComparer imageComparer) : ISlowFileGrouper
{
    private readonly AggregateFileGrouper _duplicateFileGrouper = new(
        IdenticalFileInfoGroupingFactory.Instance,
        FileLengthFileGrouper.Instance,
        new FileHashFileGrouper(fileHasher));

    public IEnumerable<IFileInfoGrouping> GroupFiles(
        IEnumerable<FileInfo> fileInfos,
        FileInfoGroupingKey? key = null)
    {
        return GroupFiles(
            fileInfos,
            FileInfoGroupingFactory.Instance,
            key);
    }

    public IEnumerable<IFileInfoGrouping> GroupFiles(
        IEnumerable<FileInfo> fileInfos,
        IFileInfoGroupingFactory groupingFactory,
        FileInfoGroupingKey? key = null)
    {
        RaiseProgressUpdate($"Starting similar images analysis for grouping {key?.ToString() ?? "unnamed"}...");
        var fileInfoArray = fileInfos.ToArray();

        if (fileInfoArray.Length < 2)
        {
            return [new FileInfoGrouping(fileInfoArray, key)];
        }

        RaiseProgressUpdate($"Seeking duplicates for grouping {key?.ToString() ?? "unnamed"}...");
        var duplicates = _duplicateFileGrouper.GroupFiles(fileInfoArray)
            .Where(x => x.Count >= 2)
            .ToArray();

        // Consider only one of each duplicate group as considering all is a waste of effort
        var filesToIgnore = duplicates
            .Select(x => x.First().FullName)
            .Distinct()
            .ToHashSet();

        var pairings = new List<(FileInfo, FileInfo[])>();
        for (var i = 0; i < fileInfoArray.Length - 1; i++)
        {
            var fileInfoI = fileInfoArray[i];
            //////////RaiseProgressUpdate($"Seeking potential pairing for file {i} of {fileInfoArray.Length} ({fileInfoI.FullName}");
            var fileBaseNameI = GetBaseFileName(fileInfoI);

            List<(FileInfo, bool)> shouldPairWith = [];

            for (var j = i + 1; j < fileInfoArray.Length; j++)
            {
                var fileInfoJ = fileInfoArray[j];
                var fileBaseNameJ = GetBaseFileName(fileInfoJ);
                if (fileBaseNameI.Equals(fileBaseNameJ, StringComparison.OrdinalIgnoreCase))
                {
                    shouldPairWith.Add((fileInfoJ, true));
                }
                else if (fileBaseNameI.Length > 3 && fileBaseNameJ.Length > 3 && NamesAreSimilarButNotIdentical(fileBaseNameI, fileBaseNameJ))
                {
                    shouldPairWith.Add((fileInfoJ, false));
                }
            }

            if (shouldPairWith.Count > 0)
            {
                pairings.Add((
                    fileInfoI,
                    shouldPairWith
                        .OrderByDescending(x => x.Item2)
                        .Select(x => x.Item1)
                        .ToArray()));
            }
        }

        RaiseFileGroupingHits(duplicates);

        if (pairings.Count == 0)
        {
            return duplicates.ToArray();
        }
        
        var result = new List<IFileInfoGrouping>(duplicates);
        int pairingIndex = 0;
        foreach (var (fileInfo1, potentialFileInfo2) in pairings)
        {
            var similarities = new List<(FileInfo, FileInfo)>();

            pairingIndex++;
            if (pairingIndex % 10 == 0)
            {
                RaiseProgressUpdate($"\tPairing {pairingIndex} of {pairings.Count}");
            }

            if (filesToIgnore.Contains(fileInfo1.FullName))
            {
                continue;
            }

            foreach (var fileInfo2 in potentialFileInfo2)
            {
                if (filesToIgnore.Contains(fileInfo2.FullName))
                {
                    continue;
                }

                // More work, but prevents memory leaks when comparing many images. The image comparer and file hasher cache images and hashes, which can consume a lot of memory.
                imageComparer.ClearIf(50);
                fileHasher.ClearIf(20000);

                var isMatch = imageComparer.Compare(
                    fileInfo1.FullName,
                    fileInfo2.FullName);
                if (!isMatch)
                {
                    continue;
                }

                // Add both files to the ignore list to reduce work. We can always re-run after dealing with duplicates/similar files to find more similar files.
                // We don't need to consider all of them at once.
                filesToIgnore.Add(fileInfo1.FullName);
                filesToIgnore.Add(fileInfo2.FullName);
                similarities.Add((fileInfo1, fileInfo2));
                RaiseFileGroupingHit(
                    groupingFactory.CreateFileInfoGrouping(
                        [fileInfo1, fileInfo2],
                        FileInfoGroupingKey.Extend(key, fileInfo1.Name)));
                break;
            }

            result.AddRange(
                similarities
                    .GroupBy(x => x.Item1.FullName)
                    .Select(g =>
                    {
                        var ret = new List<FileInfo> { g.First().Item1 };
                        ret.AddRange(g.Select(x => x.Item2));
                        return groupingFactory.CreateFileInfoGrouping(ret,
                            FileInfoGroupingKey.Extend(key, g.First().Item1.Name));
                    }));
        }

        RaiseProgressUpdate($"Ending similar images analysis for grouping {key?.ToString() ?? "unnamed"}...");
        return result.ToArray();
    }

    private static string GetBaseFileName(FileInfo fileInfo)
    {
        var fileName = fileInfo.Extension == "" ? fileInfo.Name : fileInfo.Name.Replace(fileInfo.Extension, "");
        fileName = ParenthesizedNumbersRegex().Replace(fileName, "");
        return fileName;
    }

    private static bool NamesAreSimilarButNotIdentical(string fileBaseName1, string fileBaseName2)
    {
        return HaveSimilarNames(); // || (HaveSameExtension() && HaveSimilarLength());

        bool HaveSimilarNames()
        {
            if (fileBaseName1.Equals(fileBaseName2, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (fileBaseName1.Contains(fileBaseName2, StringComparison.OrdinalIgnoreCase) ||
                fileBaseName2.Contains(fileBaseName1, StringComparison.OrdinalIgnoreCase))
            {
                return IsMatch(fileBaseName1, fileBaseName2);
            }

            return false;
        }

        static bool IsMatch(string name1, string name2)
        {
            var regex1 = IsMatchRegex(name1);
            if (regex1.IsMatch(name2))
            {
                return true;
            }

            var regex2 = IsMatchRegex(name2);
            return regex2.IsMatch(name1);
        }

        static Regex IsMatchRegex(string fileBaseName)
        {
            return new Regex($"^{fileBaseName}[_. ]+", RegexOptions.IgnoreCase);
        }

        //////////static long[] GetNumbersFromFileName(FileInfo fileInfo)
        //////////{
        //////////    return NumberRegex()
        //////////        .Matches(fileInfo.Name)
        //////////        .Select(m => long.Parse(m.Value, CultureInfo.InvariantCulture))
        //////////        .ToArray();
        //////////}

        //////////bool HaveSameExtension()
        //////////{
        //////////    return fileInfo2.Extension == "" ||
        //////////           fileInfo1.Extension.Equals(fileInfo2.Extension, StringComparison.OrdinalIgnoreCase);
        //////////}

        //////////bool HaveSimilarLength()
        //////////{
        //////////    if (fileInfo1.Length == fileInfo2.Length)
        //////////    {
        //////////        return true;
        //////////    }

        //////////    var difference = Math.Abs(fileInfo1.Length - fileInfo2.Length);
        //////////    var tolerance = Math.Max(fileInfo1.Length, fileInfo2.Length) * lengthTolerancePercent / 100;
        //////////    return difference <= tolerance;
        //////////}
    }

    //////////[GeneratedRegex(@"\d{2,}")]
    //////////private static partial Regex NumberRegex();

    [GeneratedRegex(@"\s*[(]\d+[)]")]
    private static partial Regex ParenthesizedNumbersRegex();

    public event EventHandler<FileGroupingHitEventArgs>? FileGroupingHit;
    public event EventHandler<string>? ProgressUpdate;

    private void RaiseFileGroupingHit(IFileInfoGrouping fileInfoGrouping)
    {
        FileGroupingHit?.Invoke(this, new FileGroupingHitEventArgs(fileInfoGrouping));
    }

    private void RaiseProgressUpdate(string progressUpdate)
    {
        ProgressUpdate?.Invoke(this, progressUpdate);
    }

    private void RaiseFileGroupingHits(IFileInfoGrouping[] fileInfoGroupings)
    {
        foreach (var fileInfoGrouping in fileInfoGroupings)
        {
            RaiseFileGroupingHit(fileInfoGrouping);
        }
    }
}
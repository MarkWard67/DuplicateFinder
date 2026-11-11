using System.Collections.Concurrent;
using DuplicateFinder.FilePairers;
using DuplicateFinder.Files.FileGroupings;
using DuplicateFinder.Files.FileHashers;
using DuplicateFinder.ImageComparers;

namespace DuplicateFinder.Files.FileGroupers;

internal class SimilarImagesFileGrouper(
    IFileHasher fileHasher,
    IImageComparer imageComparer,
    IFilePairer filePairer) : ISlowFileGrouper
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

        try
        {
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
            var filesToIgnore = new ConcurrentDictionary<string, byte>(
                duplicates
                    .SelectMany(x => x.Skip(1))
                    .Select(x => x.FullName)
                    .Distinct()
                    .Select(x => new KeyValuePair<string, byte>(x, 0)));

            var pairings = filePairer
                .PairFiles(fileInfoArray)
                .OrderByDescending(x => x.PairedFileInfos.Length)
                .ToArray();

            RaiseFileGroupingHits(duplicates);

            if (pairings.Length == 0)
            {
                return duplicates.ToArray();
            }

            var result = new List<IFileInfoGrouping>(duplicates);
            var pairingIndex = 0;
            foreach (var (fileInfo1, potentialFileInfo2) in pairings)
            {
                var similarities = new List<(FileInfo, FileInfo)>();

                pairingIndex++;
                if (pairingIndex % 10 == 0)
                {
                    RaiseProgressUpdate($"Pairing {pairingIndex} of {pairings.Length}");
                }

                if (filesToIgnore.ContainsKey(fileInfo1.FullName))
                {
                    continue;
                }

                var cancellationTokenSource = new CancellationTokenSource();
                var cancellationToken = cancellationTokenSource.Token;

                var similarFileTasks =
                    potentialFileInfo2
                        .AsParallel()
                        .WithDegreeOfParallelism(Environment.ProcessorCount)
                        .Select(async f =>
                        {
                            if (cancellationToken.IsCancellationRequested || filesToIgnore.ContainsKey(f.FullName))
                            {
                                return (f, false);
                            }

                            return (f, await imageComparer.CompareAsync(fileInfo1.FullName, f.FullName, cancellationToken));
                        });

                var similarFiles = Task.WhenAll(similarFileTasks).GetAwaiter().GetResult()
                        .Where(x => x.Item2)
                        .Select(x => x.f)
                        .ToArray();

                if (similarFiles.Length > 0)
                {
                    filesToIgnore.AddOrUpdate(fileInfo1.FullName, _ => 0, (_, _) => 0);
                }

                foreach (var similarFile in similarFiles)
                {
                    filesToIgnore.AddOrUpdate(similarFile.FullName, _ => 0, (_, _) => 0);
                    similarities.Add((fileInfo1, similarFile));
                    RaiseFileGroupingHit(
                        groupingFactory.CreateFileInfoGrouping(
                            [fileInfo1, similarFile],
                            FileInfoGroupingKey.Extend(key, fileInfo1.Name)));
                }

                result.AddRange(
                    similarities
                        .GroupBy(x => x.Item1.FullName)
                        .Select(g =>
                        {
                            var ret = new List<FileInfo> { g.First().Item1 };
                            ret.AddRange(g.Select(x => x.Item2));
                            return groupingFactory.CreateFileInfoGrouping(
                                ret,
                                FileInfoGroupingKey.Extend(key, g.First().Item1.Name));
                        }));
            }
            
            return result.ToArray();
        }
        finally
        {
            RaiseProgressUpdate($"Ending similar images analysis for grouping {key?.ToString() ?? "unnamed"}...");
        }
    }

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

    public void Dispose()
    {
        _duplicateFileGrouper.Dispose();
        fileHasher.Dispose();
        imageComparer.Dispose();
    }
}
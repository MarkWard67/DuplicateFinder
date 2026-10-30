using DuplicateFinder.Files.FileGroupings;

namespace DuplicateFinder.Files.FileGroupers;

internal class SimilarImagesFileGrouper(
    IFileHasher fileHasher,
    IImageComparer imageComparer) : IFileGrouper
{
    private readonly AggregateFileGrouper _duplicateFileGrouper = new(
        IdenticalFileInfoGroupingFactory.Instance,
        FileLengthFileGrouper.Instance,
        new FileHashFileGrouper(fileHasher));

    public IEnumerable<IFileInfoGrouping> GroupFiles(
        IEnumerable<FileInfoWrapper> fileInfos,
        FileInfoGroupingKey? key = null)
    {
        return GroupFiles(
            fileInfos,
            FileInfoGroupingFactory.Instance,
            key);
    }

    public IEnumerable<IFileInfoGrouping> GroupFiles(
        IEnumerable<FileInfoWrapper> fileInfos,
        IFileInfoGroupingFactory groupingFactory,
        FileInfoGroupingKey? key = null)
    {
        var fileInfoArray = fileInfos.ToArray();

        if (fileInfoArray.Length < 2)
        {
            return [new FileInfoGrouping(fileInfoArray, key)];
        }

        var duplicates = _duplicateFileGrouper.GroupFiles(fileInfoArray)
            .Where(x => x.Count >= 2)
            .ToArray();

        // Consider only one of each duplicate group as considering all is a waste of effort
        var filesToIgnore = duplicates
                .Select(x => x.First().FileInfo.FullName)
                .Distinct()
                .ToHashSet();

        var result = new List<IFileInfoGrouping>(duplicates);

        for (var i = 0; i < fileInfoArray.Length - 1; i++)
        {
            var fileInfoI = fileInfoArray[i];
            if (filesToIgnore.Contains(fileInfoI.FileInfo.FullName))
            {
                continue;
            }

            var similarToI = new List<FileInfoWrapper> { fileInfoI };
            for (var j = i + 1; j < fileInfoArray.Length; j++)
            {
                var fileInfoJ = fileInfoArray[j];

                if (filesToIgnore.Contains(fileInfoJ.FileInfo.FullName))
                {
                    continue;
                }

                var isMatch = imageComparer.Compare(
                    fileInfoI.FileInfo.FullName,
                    fileInfoJ.FileInfo.FullName);
                if (!isMatch)
                {
                    continue;
                }

                // Add both files to the ignore list to reduce work. We can always re-run after dealing with duplicates/similar files to find more similar files.
                // We don't need to consider all of them at once.
                filesToIgnore.Add(fileInfoI.FileInfo.FullName);
                filesToIgnore.Add(fileInfoJ.FileInfo.FullName);
                similarToI.Add(fileInfoJ);
            }

            if (similarToI.Count > 1)
            {
                result.Add(
                    groupingFactory.CreateFileInfoGrouping(
                        similarToI,
                        FileInfoGroupingKey.Extend(key, fileInfoI.FileInfo.FullName)));
            }
        }

        return result.ToArray();
    }

    public void Dispose()
    {
        imageComparer.Dispose();
    }
}
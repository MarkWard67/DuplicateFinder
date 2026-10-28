using DuplicateFinder.Files.FileGroupings;

namespace DuplicateFinder.Files.FileGroupers;

internal class SimilarImagesFileGrouper(IFileHasher fileHasher) : IFileGrouper
{
    private readonly DeduplicatingFileGrouper _duplicateFileGrouper = new(fileHasher);

    public IEnumerable<IFileInfoGrouping> GroupFiles(IEnumerable<FileInfoWrapper> fileInfos)
    {
        var fileInfoArray = fileInfos.ToArray();

        if (fileInfoArray.Length < 2)
        {
            return [new FileInfoGrouping(fileInfoArray)];
        }

        var duplicates = _duplicateFileGrouper.GroupFiles(fileInfoArray)
            .Where(x => x.Count >= 2)
            .ToArray();

        var result = new List<IFileInfoGrouping>(duplicates);
        using var imageComparer = new ImageComparer(fileHasher);

        for (var i = 0; i < fileInfoArray.Length - 1; i++)
        {
            var fileInfoI = fileInfoArray[i];
            var similarToI = new List<FileInfoWrapper>();
            for (var j = i + 1; j < fileInfoArray.Length; j++)
            {
                var fileInfoJ = fileInfoArray[j];

                if (duplicates.Any(x => x.ContainsAll(fileInfoI, fileInfoJ)))
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

                similarToI.Add(fileInfoJ);
                break;
            }

            if (similarToI.Count > 0)
            {
                result.Add(
                    new SimilarFileInfoGrouping(
                        fileInfoI,
                        similarToI)
                );
            }
        }

        return result.ToArray();
    }
}
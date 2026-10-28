using DuplicateFinder.Files.FileGroupings;

namespace DuplicateFinder.Files.FileGroupers;

internal class SimilarImagesFileGrouper(IFileHasher fileHasher) : IFileGrouper
{
    public IEnumerable<IFileInfoGrouping> GroupFiles(IEnumerable<FileInfoWrapper> fileInfos)
    {
        var fileInfoArray = fileInfos.ToArray();
        if (fileInfoArray.Length < 2) return [new FileInfoGrouping(fileInfoArray)];

        var result = new List<IFileInfoGrouping>();
        using var imageComparer = new ImageComparer(fileHasher);

        for (var i = 0; i < fileInfoArray.Length - 1; i++)
        {
            var fileInfoI = fileInfoArray[i];
            var similarToI = new List<FileInfoWrapper>();
            for (var j = i + 1; j < fileInfoArray.Length; j++)
            {
                var isMatch = imageComparer.Compare(
                    fileInfoI.FileInfo.FullName,
                    fileInfoArray[j].FileInfo.FullName);
                if (!isMatch) continue;

                similarToI.Add(fileInfoArray[j]);
                break;
            }

            if (similarToI.Count > 0)
                result.Add(
                    new SimilarFileInfoGrouping(
                        fileInfoI,
                        similarToI)
                );
        }

        return result.ToArray();
    }
}
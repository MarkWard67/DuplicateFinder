namespace DuplicateFinder.FilePairers;

internal class DefaultFilePairer : IFilePairer
{
    private DefaultFilePairer()
    {
    }

    public static DefaultFilePairer Instance { get; } = new();

    public IEnumerable<FileInfoPairing> PairFiles(IEnumerable<FileInfo> fileInfos)
    {
        var fileInfoArray = fileInfos.ToArray();

        var pairings = new List<FileInfoPairing>();
        for (var i = 0; i < fileInfoArray.Length - 1; i++)
        {
            var fileInfoI = fileInfoArray[i];

            List<FileInfo> shouldPairWith = [];
            for (var j = i + 1; j < fileInfoArray.Length; j++)
            {
                var fileInfoJ = fileInfoArray[j];
                shouldPairWith.Add(fileInfoJ);
            }

            if (shouldPairWith.Count > 0)
            {
                pairings.Add(
                    new FileInfoPairing(
                        fileInfoI,
                        shouldPairWith));
            }
        }

        return pairings;
    }
}
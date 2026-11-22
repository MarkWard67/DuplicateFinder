namespace DuplicateFinder.Files.FilePairers;

internal abstract class FilePairer : IFilePairer
{
    private readonly string? _redundantFilesFolderPath;

    protected FilePairer()
    {
    }

    protected FilePairer(string redundantFilesFolderPath)
    {
        _redundantFilesFolderPath = redundantFilesFolderPath;
    }

    public virtual IEnumerable<FileInfoPairing> PairFiles(IEnumerable<FileInfo> fileInfos)
    {
        if (_redundantFilesFolderPath == null)
        {
            return PairAllFiles(fileInfos);
        }

        var fileInfosByRedundancy = fileInfos
            .GroupBy(x => x.DirectoryName == _redundantFilesFolderPath)
            .ToDictionary(
                x => x.Key,
                x => x.ToArray());

        if (!fileInfosByRedundancy.TryGetValue(true, out var redundantFiles))
        {
            return [];
        }

        return fileInfosByRedundancy.TryGetValue(false, out var nonRedundantFiles)
            ? PairFiles(redundantFiles, (_, _) => nonRedundantFiles)
            : PairAllFiles(redundantFiles);


        IEnumerable<FileInfoPairing> PairAllFiles(IEnumerable<FileInfo> fileInfosToPair)
        {
            return PairFiles(fileInfosToPair, SkipI);
        }
    }

    private static IEnumerable<FileInfo> SkipI(IEnumerable<FileInfo> f, int i)
    {
        return f.Skip(i + 1);
    }

    protected virtual IEnumerable<FileInfoPairing> PairFiles(
        IEnumerable<FileInfo> fileInfosOuter,
        Func<IEnumerable<FileInfo>, int, IEnumerable<FileInfo>> fileInfosInnerSelector)
    {
        var outerFileInfoArray = fileInfosOuter.ToArray();

        var pairings = new List<FileInfoPairing>();
        for (var i = 0; i < outerFileInfoArray.Length; i++)
        {
            var fileInfoI = outerFileInfoArray[i];

            List<FileInfo> pairWith = [];
            var innerFilesArray = fileInfosInnerSelector(outerFileInfoArray, i).ToArray();
            foreach (var fileInfoJ in innerFilesArray)
            {
                var shouldPair = ShouldPair(fileInfoI, fileInfoJ);
                if (shouldPair)
                {
                    pairWith.Add(fileInfoJ);
                }
            }

            if (pairWith.Count > 0)
            {
                pairings.Add(
                    new FileInfoPairing(
                        fileInfoI,
                        pairWith));
            }
        }

        return pairings;
    }

    protected abstract bool ShouldPair(FileInfo fileInfo1, FileInfo fileInfo2);

    public virtual void Dispose()
    {
        // TODO release managed resources here
    }
}
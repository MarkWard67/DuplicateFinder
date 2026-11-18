namespace DuplicateFinder.Files.FilePairers;

internal class SimilarFilesFilePairer : FilePairer
{
    private readonly IFilePairer[] _filePairers;

    public SimilarFilesFilePairer()
    {
        _filePairers =
        [
            new FileNameFilePairer(),
            new FilePartialNameFilePairer()
            //////////new DateTimeTakenFilePairer((h, m, s) => new TimeOnly(h, m, s)),
            //////////new DateTimeTakenFilePairer((h, m, _) => new TimeOnly(h, m, 0)),
            //////////new DateTimeTakenFilePairer((h, _, _) => new TimeOnly(h, 0, 0))
        ];
    }

    public SimilarFilesFilePairer(string redundantFilesFolderPath)
        : base(redundantFilesFolderPath)
    {
        _filePairers =
        [
            new FileNameFilePairer(redundantFilesFolderPath),
            new FilePartialNameFilePairer(redundantFilesFolderPath)
            //////////new DateTimeTakenFilePairer(redundantFilesFolderPath, (h, m, s) => new TimeOnly(h, m, s)),
            //////////new DateTimeTakenFilePairer(redundantFilesFolderPath, (h, m, _) => new TimeOnly(h, m, 0)),
            //////////new DateTimeTakenFilePairer(redundantFilesFolderPath, (h, _, _) => new TimeOnly(h, 0, 0))

        ];
    }

    protected override bool ShouldPair(FileInfo fileInfo1, FileInfo fileInfo2)
    {
        return !fileInfo1.FullName.Equals(fileInfo2.FullName, StringComparison.OrdinalIgnoreCase);
    }

    public override IEnumerable<FileInfoPairing> PairFiles(IEnumerable<FileInfo> fileInfos)
    {
        var result = new List<FileInfoPairing>();

        var fileInfoArray = fileInfos.ToArray();
        var fileNamesToIgnore = new HashSet<string>();

        foreach (var filePairer in _filePairers)
        {
            var pairings = filePairer.PairFiles(fileInfoArray).ToList();
            result.AddRange(pairings);

            var pairedFileNames = pairings
                .SelectMany(x =>
                {
                    var ret = new List<FileInfo> { x.MasterFileInfo };
                    ret.AddRange(x.PairedFileInfos);
                    return ret;
                })
                .Select(f => f.FullName)
                .Distinct();

            foreach (var pairedFileName in pairedFileNames)
            {
                fileNamesToIgnore.Add(pairedFileName);
            }

            fileInfoArray = fileInfoArray.Where(x => !fileNamesToIgnore.Contains(x.FullName)).ToArray();
        }

        //////////REINSTATE
        //////////result.AddRange(base.PairFiles(fileInfoArray)); -- reinstate
        return result;
    }

    //////////protected override IEnumerable<FileInfoPairing> PairFiles(IEnumerable<FileInfo> fileInfosOuter, Func<IEnumerable<FileInfo>, int, IEnumerable<FileInfo>> fileInfosInnerSelector)
    //////////{
    //////////    var fileNamePairings = _fileNameFilePairer.PairFiles(fileInfosOuter, fileInfosInnerSelector);


    //////////    return base.PairFiles(fileInfosOuter, fileInfosInnerSelector);
    //////////}

    //////////public IEnumerable<FileInfoPairing> PairFiles(IEnumerable<FileInfo> fileInfos)
    //////////{
    //////////    if (_redundantFilesFolderPath == null)
    //////////    {
    //////////        return PairAllFiles(fileInfos);
    //////////    }

    //////////    var fileInfosByRedundancy = fileInfos
    //////////        .GroupBy(x => x.DirectoryName == _redundantFilesFolderPath)
    //////////        .ToDictionary(
    //////////            x => x.Key,
    //////////            x => x.ToArray());

    //////////    if (!fileInfosByRedundancy.TryGetValue(true, out var redundantFiles))
    //////////    {
    //////////        return [];
    //////////    }

    //////////    return fileInfosByRedundancy.TryGetValue(false, out var nonRedundantFiles)
    //////////        ? PairFiles(redundantFiles, (_, _) => nonRedundantFiles)
    //////////        : PairAllFiles(redundantFiles);


    //////////    static IEnumerable<FileInfoPairing> PairAllFiles(IEnumerable<FileInfo> fileInfos)
    //////////    {
    //////////        return PairFiles(fileInfos, (f, i) => f.Skip(i));
    //////////    }
    //////////}

    ////////////////////private static List<FileInfoPairing> PairAllFiles(IEnumerable<FileInfo> fileInfos)
    ////////////////////{
    ////////////////////    var fileInfosArray = fileInfos.ToArray();
    ////////////////////    return PairFiles(
    ////////////////////        fileInfosArray,
    ////////////////////        (f, i) => f.Skip(i));

    ////////////////////    //////////var fileInfoArray = fileInfos.ToArray();

    ////////////////////    //////////var pairings = new List<FileInfoPairing>();
    ////////////////////    //////////for (var i = 0; i < fileInfoArray.Length - 1; i++)
    ////////////////////    //////////{
    ////////////////////    //////////    var fileInfoI = fileInfoArray[i];

    ////////////////////    //////////    List<(FileInfo, bool)> shouldPairWith = [];

    ////////////////////    //////////    for (var j = i + 1; j < fileInfoArray.Length; j++)
    ////////////////////    //////////    {
    ////////////////////    //////////        var fileInfoJ = fileInfoArray[j];
    ////////////////////    //////////        var shouldPair = ShouldPair(fileInfoI, fileInfoJ);
    ////////////////////    //////////        if (shouldPair.HasValue)
    ////////////////////    //////////        {
    ////////////////////    //////////            shouldPairWith.Add((fileInfoJ, shouldPair.Value));
    ////////////////////    //////////        }
    ////////////////////    //////////    }

    ////////////////////    //////////    if (shouldPairWith.Count > 0)
    ////////////////////    //////////    {
    ////////////////////    //////////        pairings.Add(
    ////////////////////    //////////            new FileInfoPairing(
    ////////////////////    //////////                fileInfoI,
    ////////////////////    //////////                shouldPairWith
    ////////////////////    //////////                    .OrderByDescending(x => x.Item2)
    ////////////////////    //////////                    .Select(x => x.Item1)));
    ////////////////////    //////////    }
    ////////////////////    //////////}

    ////////////////////    //////////return pairings;
    ////////////////////}


    ////////////////////private static List<FileInfoPairing> PairFiles(
    ////////////////////    IEnumerable<FileInfo> redundantFileInfos,
    ////////////////////    IEnumerable<FileInfo> nonRedundantFileInfos)
    ////////////////////{
    ////////////////////    var redundantFileInfoArray = redundantFileInfos.ToArray();
    ////////////////////    return PairFiles(redundantFileInfoArray, (_, _) => nonRedundantFileInfos);

    ////////////////////    //////////var nonRedundantFileInfoArray = nonRedundantFileInfos.ToArray();

    ////////////////////    //////////var pairings = new List<FileInfoPairing>();
    ////////////////////    //////////for (var i = 0; i < redundantFileInfoArray.Length; i++)
    ////////////////////    //////////{
    ////////////////////    //////////    var fileInfoI = redundantFileInfoArray[i];

    ////////////////////    //////////    List<(FileInfo, bool)> shouldPairWith = [];

    ////////////////////    //////////    foreach (var fileInfoJ in nonRedundantFileInfoArray)
    ////////////////////    //////////    {
    ////////////////////    //////////        var shouldPair = ShouldPair(fileInfoI, fileInfoJ);
    ////////////////////    //////////        if (shouldPair.HasValue)
    ////////////////////    //////////        {
    ////////////////////    //////////            shouldPairWith.Add((fileInfoJ, shouldPair.Value));
    ////////////////////    //////////        }
    ////////////////////    //////////    }

    ////////////////////    //////////    if (shouldPairWith.Count > 0)
    ////////////////////    //////////    {
    ////////////////////    //////////        pairings.Add(
    ////////////////////    //////////            new FileInfoPairing(
    ////////////////////    //////////                fileInfoI,
    ////////////////////    //////////                shouldPairWith
    ////////////////////    //////////                    .OrderByDescending(x => x.Item2)
    ////////////////////    //////////                    .Select(x => x.Item1)));
    ////////////////////    //////////    }
    ////////////////////    //////////}

    ////////////////////    //////////return pairings;
    ////////////////////}

    //////////private static List<FileInfoPairing> PairFiles(
    //////////    IEnumerable<FileInfo> fileInfosOuter,
    //////////    Func<IEnumerable<FileInfo>, int, IEnumerable<FileInfo>> fileInfosInnerSelector)
    //////////{
    //////////    var outerFileInfoArray = fileInfosOuter.ToArray();

    //////////    var pairings = new List<FileInfoPairing>();
    //////////    for (var i = 0; i < outerFileInfoArray.Length; i++)
    //////////    {
    //////////        var fileInfoI = outerFileInfoArray[i];

    //////////        List<(FileInfo, bool)> shouldPairWith = [];
    //////////        var innerFilesArray = fileInfosInnerSelector(outerFileInfoArray, i).ToArray();
    //////////        foreach (var fileInfoJ in innerFilesArray)
    //////////        {
    //////////            var shouldPair = ShouldPair(fileInfoI, fileInfoJ);
    //////////            if (shouldPair.HasValue)
    //////////            {
    //////////                shouldPairWith.Add((fileInfoJ, shouldPair.Value));
    //////////            }
    //////////        }

    //////////        if (shouldPairWith.Count > 0)
    //////////        {
    //////////            pairings.Add(
    //////////                new FileInfoPairing(
    //////////                    fileInfoI,
    //////////                    shouldPairWith
    //////////                        .OrderByDescending(x => x.Item2)
    //////////                        .Select(x => x.Item1)));
    //////////        }
    //////////    }

    //////////    return pairings;
    //////////}


    //////////private static bool? ShouldPair(FileInfo fileInfo1, FileInfo fileInfo2)
    //////////{
    //////////    if (fileInfo1.Name.Equals(fileInfo2.Name, StringComparison.OrdinalIgnoreCase))
    //////////    {
    //////////        return true;
    //////////    }

    //////////    var dateTimeTaken1 = GetDateTimeTaken(fileInfo1);
    //////////    var dateTimeTaken2 = GetDateTimeTaken(fileInfo2);
    //////////    if (dateTimeTaken1 != null && dateTimeTaken2 != null)
    //////////    {
    //////////        var difference = dateTimeTaken1 > dateTimeTaken2
    //////////            ? dateTimeTaken1.Value.Subtract(dateTimeTaken2.Value)
    //////////            : dateTimeTaken2.Value.Subtract(dateTimeTaken1.Value);

    //////////        return difference < TimeSpan.FromHours(1);
    //////////    }

    //////////    var fileBaseName1 = GetBaseFileName(fileInfo1);
    //////////    var fileBaseName2 = GetBaseFileName(fileInfo2);
    //////////    return NamesAreSimilar(fileBaseName1, fileBaseName2);
    //////////}

    ////////////////////private static DateTime? GetDateTimeTaken(FileInfo fileInfo)
    ////////////////////{
    ////////////////////    var matches = DateTimeRegex1().Matches(fileInfo.Name);
    ////////////////////    if (!matches.Any())
    ////////////////////    {
    ////////////////////        matches = DateTimeRegex2().Matches(fileInfo.Name);
    ////////////////////    }

    ////////////////////    foreach (Match match in matches)
    ////////////////////    {
    ////////////////////        var year = int.Parse(match.Groups["year"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture);
    ////////////////////        var month = int.Parse(match.Groups["month"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture);
    ////////////////////        var day = int.Parse(match.Groups["day"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture);
    ////////////////////        var hour = match.Groups["hour"].Success
    ////////////////////            ? int.Parse(match.Groups["hour"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture)
    ////////////////////            : 0;
    ////////////////////        var minute = match.Groups["minute"].Success
    ////////////////////            ? int.Parse(match.Groups["minute"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture)
    ////////////////////            : 0;
    ////////////////////        var second = match.Groups["second"].Success
    ////////////////////            ? int.Parse(match.Groups["second"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture)
    ////////////////////            : 0;

    ////////////////////        if (year >= 1966 && year <= DateTime.Now.Year && month is >= 1 and <= 12 && day is >= 1 and <= 31)
    ////////////////////        {
    ////////////////////            try
    ////////////////////            {
    ////////////////////                return new DateTime(
    ////////////////////                    new DateOnly(year, month, day),
    ////////////////////                    new TimeOnly(hour, minute, second),
    ////////////////////                    DateTimeKind.Utc);
    ////////////////////            }
    ////////////////////            catch (Exception ex)
    ////////////////////            {
    ////////////////////                return null;
    ////////////////////            }
    ////////////////////        }
    ////////////////////    }

    ////////////////////    return null;
    ////////////////////}

    ////////////////////[GeneratedRegex(
    ////////////////////    @"(?<year>\d{4})-(?<month>\d{2})-(?<day>\d{2})(\s+(?<hour>\d{2})-(?<minute>\d{2})(-(?<second>\d{2}))?)?")]
    ////////////////////private static partial Regex DateTimeRegex1();

    ////////////////////[GeneratedRegex(
    ////////////////////    @"(?<year>\d{4})(?<month>\d{2})(?<day>\d{2})(_(?<hour>\d{2})(?<minute>\d{2})((?<second>\d{2}))?)?")]
    ////////////////////private static partial Regex DateTimeRegex2();

    //////////private static string GetBaseFileName(FileInfo fileInfo)
    //////////{
    //////////    var fileName = fileInfo.Extension == "" ? fileInfo.Name : fileInfo.Name.Replace(fileInfo.Extension, "");
    //////////    fileName = ParenthesizedNumbersRegex().Replace(fileName, "");
    //////////    return fileName;
    //////////}

    //////////private static bool NamesAreSimilar(string fileBaseName1, string fileBaseName2)
    //////////{
    //////////    if (fileBaseName1.Equals(fileBaseName2, StringComparison.OrdinalIgnoreCase))
    //////////    {
    //////////        return true;
    //////////    }

    //////////    if (fileBaseName1.Contains(fileBaseName2, StringComparison.OrdinalIgnoreCase) ||
    //////////        fileBaseName2.Contains(fileBaseName1, StringComparison.OrdinalIgnoreCase))
    //////////    {
    //////////        return IsMatch(fileBaseName1, fileBaseName2);
    //////////    }

    //////////    return false;

    //////////    static bool IsMatch(string name1, string name2)
    //////////    {
    //////////        var regex1 = IsMatchRegex(name1);
    //////////        if (regex1.IsMatch(name2))
    //////////        {
    //////////            return true;
    //////////        }

    //////////        var regex2 = IsMatchRegex(name2);
    //////////        return regex2.IsMatch(name1);
    //////////    }

    //////////    static Regex IsMatchRegex(string fileBaseName)
    //////////    {
    //////////        return new Regex($"^{fileBaseName}[_. ]+", RegexOptions.IgnoreCase);
    //////////    }
    //////////}

    public override void Dispose()
    {
        foreach (var filePairer in _filePairers)
        {
            filePairer.Dispose();
        }

        base.Dispose();
    }
}
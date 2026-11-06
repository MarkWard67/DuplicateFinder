using System.Text.RegularExpressions;

namespace DuplicateFinder.FilePairers;

internal partial class SimilarFilesFilePairer : IFilePairer
{
    private SimilarFilesFilePairer()
    {
    }

    public static SimilarFilesFilePairer Instance { get; } = new();

    public IEnumerable<FileInfoPairing> PairFiles(IEnumerable<FileInfo> fileInfos)
    {
        var fileInfoArray = fileInfos.ToArray();

        var pairings = new List<FileInfoPairing>();
        for (var i = 0; i < fileInfoArray.Length - 1; i++)
        {
            var fileInfoI = fileInfoArray[i];
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
                else if (fileBaseNameI.Length > 3 && fileBaseNameJ.Length > 3 &&
                         NamesAreSimilarButNotIdentical(fileBaseNameI, fileBaseNameJ))
                {
                    shouldPairWith.Add((fileInfoJ, false));
                }
            }

            if (shouldPairWith.Count > 0)
            {
                pairings.Add(new FileInfoPairing(
                    fileInfoI,
                    shouldPairWith
                        .OrderByDescending(x => x.Item2)
                        .Select(x => x.Item1)));
            }
        }

        return pairings;
    }

    [GeneratedRegex(@"\s*[(]\d+[)]")]
    private static partial Regex ParenthesizedNumbersRegex();

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
    }
}
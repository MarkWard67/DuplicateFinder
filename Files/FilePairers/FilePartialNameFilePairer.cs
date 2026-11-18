using System.Text.RegularExpressions;
using DuplicateFinder.Caches;

namespace DuplicateFinder.Files.FilePairers;

internal partial class FilePartialNameFilePairer : FilePairer
{
    private readonly FileBaseNameCache _baseNameCache = new();

    public FilePartialNameFilePairer()
    {
    }

    public FilePartialNameFilePairer(string redundantFilesFolderPath)
        : base(redundantFilesFolderPath)
    {
    }

    protected override bool ShouldPair(FileInfo fileInfo1, FileInfo fileInfo2)
    {
        var baseName1 = _baseNameCache.GetOrCreateAsync(
                fileInfo1.FullName,
                _ => Task.Run(() => GetBaseFileName(fileInfo1)),
                CancellationToken.None)
            .GetAwaiter()
            .GetResult();

        var baseName2 = _baseNameCache.GetOrCreateAsync(
                fileInfo2.FullName,
                _ => Task.Run(() => GetBaseFileName(fileInfo2)),
                CancellationToken.None)
            .GetAwaiter()
            .GetResult();

        return baseName1.Length > 3 &&
               baseName2.Length > 3 && (
                   baseName1.Contains(baseName2, StringComparison.OrdinalIgnoreCase) ||
                    baseName2.Contains(baseName1, StringComparison.OrdinalIgnoreCase));
    }

    private static string GetBaseFileName(FileInfo fileInfo)
    {
        var fileName = fileInfo.Extension == "" ? fileInfo.Name : fileInfo.Name.Replace(fileInfo.Extension, "");
        fileName = ParenthesizedNumbersRegex().Replace(fileName, "");
        return fileName;
    }

    public override void Dispose()
    {
        _baseNameCache.Dispose();
        base.Dispose();
    }


    [GeneratedRegex(@"\s*[(]\d+[)]")]
    private static partial Regex ParenthesizedNumbersRegex();

    private class FileBaseNameCache() : Cache<string, string>(new SemaphoreSlimCache<string>());
}
using System.IO.Compression;

namespace DuplicateFinder;

internal class ZipFileExtractor(
    string targetPath,
    params string[] zipFileFullNames)
{
    public ZipFileExtractor(
        string targetPath,
        string zipPath,
        string zipFileSpec)
        : this(
            targetPath,
            Directory.GetFiles(zipPath, zipFileSpec, SearchOption.TopDirectoryOnly))
    {
    }

    public void Extract(bool overwriteFiles = false)
    {
        if (!Directory.Exists(targetPath)) Directory.CreateDirectory(targetPath);

        foreach (var zipFileFullName in zipFileFullNames)
        {
            ZipFile.ExtractToDirectory(zipFileFullName, targetPath, overwriteFiles);
            OnProgressChange(new StringEventArgs($"Extracted: {zipFileFullName}"));
        }
    }

    public event EventHandler<StringEventArgs>? ProgressChanged;

    private void OnProgressChange(StringEventArgs e)
    {
        ProgressChanged?.Invoke(this, e);
    }
}
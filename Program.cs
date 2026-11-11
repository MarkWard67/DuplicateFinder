using DuplicateFinder;
using DuplicateFinder.FileGroupingReporters;
using DuplicateFinder.FilePairers;
using DuplicateFinder.Files;
using DuplicateFinder.Files.FileExcluders;
using DuplicateFinder.Files.FileFinders;
using DuplicateFinder.Files.FileGroupers;
using DuplicateFinder.Files.FileHashers;
using DuplicateFinder.Folders;
using DuplicateFinder.Folders.FolderFinders;
using DuplicateFinder.ImageComparers;
using DuplicateFinder.Reporters;
using TextCopy;

const string rootFolder = @"G:\";

var commandType = GetCommandType();

var filePairer = new SimilarFilesFilePairer(Path.Combine(rootFolder, @"Other computers\Office PC\Photos\From Google Photos 20261001"));

using var cachingSha256FileHasher = new CachingSha256FileHasher();
using var imageComparer = new ImageMagickFallbackImageComparer(1e-2, ImageMagickImageMemoryCache.Instance);

//////////var xxx = imageComparer.Compare(
//////////@"G:\Other computers\Office PC\Photos\From Google Photos 20261001\2016-02-21 12-19.jpg",
//////////@"G:\Other computers\Office PC\Photos\Places\UK\Salisbury\2016-02-21 12-19.jpg");

IFileGroupingReporter? reporter = null;

try
{
    switch (commandType)
    {
        case CommandType.ExtractZipFiles:
            ExtractZipFiles("takeout-20261008*.zip");
            break;

        case CommandType.FindDuplicatesWithinFolders:
            // Handle finding duplicates in individual folders
            reporter = FindAndReportDuplicatesWithinFolders(
                cachingSha256FileHasher,
                [Path.Combine(rootFolder, @"Other computers\Office PC")],
                DefaultReporter.Instance);
            break;

        case CommandType.FindSimilarFilesWithinFolders:
            // Handle finding similar files in individual folders
            reporter = FindAndReportSimilarFilesInIndividualFolders(
                cachingSha256FileHasher,
                imageComparer,
                [Path.Combine(rootFolder, @"Other computers\Office PC")],
                DefaultReporter.Instance,
                filePairer);
            break;

        case CommandType.FindDuplicatesAcrossFolders:
            // Handle finding duplicates across folder structures
            reporter = FindAndReportDuplicateAcrossFolders(
                cachingSha256FileHasher,
                [Path.Combine(rootFolder, @"Other computers\Office PC")],
                DefaultReporter.Instance);
            break;

        case CommandType.FindSimilarFilesAcrossFolders:
            // Handle finding similar files across folder structures
            reporter = FindAndReportSimilarFilesAcrossFolders(
                cachingSha256FileHasher,
                imageComparer,
                [Path.Combine(rootFolder, @"Other computers\Office PC")],
                DefaultReporter.Instance,
                filePairer);
            break;

        case CommandType.DeleteSimilarFilesWithinSpecificFolders:
            // Delete duplicate files and similar files within specific folders
            // If the files to which they are similar are in a different folder.
            reporter = FindAndReportSimilarFilesAcrossFolders(
                cachingSha256FileHasher,
                imageComparer,
                [Path.Combine(rootFolder, @"Other computers\Office PC")],
                new EraseFilesReporter(
                    Path.Combine(rootFolder, @"Other computers\Office PC\Photos\From Google Photos 20261001"),
                    Path.Combine(rootFolder, @"Other computers\Office PC\Photos\__MISCELLANEOUS"),
                    Path.Combine(rootFolder, @"Other computers\Office PC\Photos\Years\*"),
                    Path.Combine(rootFolder, @"Other computers\Office PC\Photos\Printed")),
                filePairer);
            break;

        default:
            throw new ArgumentOutOfRangeException();
    }

    reporter?.SubReportAvailable += OnSubReportAvailable;
    reporter?.ProgressUpdate += OnProgressUpdate;

    var report = reporter?.Report();

    if (report == null)
    {
        Console.WriteLine("ERROR! NO REPORT");
        return 1;
    }

    ClipboardService.SetText(report);
    Console.WriteLine(report);
}
finally
{
    reporter?.Dispose();
}

return 0;

static void OnSubReportAvailable(object? sender, SubReportEventArgs e)
{
    var color = Console.ForegroundColor;
    try
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(e.SubReport);
    }
    finally
    {
        Console.ForegroundColor = color;
    }
}

static void OnProgressUpdate(object? sender, string e)
{
    var color = Console.ForegroundColor;
    try
    {
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write($"{DateTime.Now:G} ");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(e);
    }
    finally
    {
        Console.ForegroundColor = color;
    }
}

static CommandType GetCommandType()
{
    return CommandType.DeleteSimilarFilesWithinSpecificFolders;
}

static void ExtractZipFiles(string zipFileSpec)
{
    var zipFileExtractor = new ZipFileExtractor(
        rootFolder,
        KnownFolders.GetPath(KnownFolder.Downloads),
        zipFileSpec);
    zipFileExtractor.ProgressChanged += (_, e) => Console.WriteLine(e.Value);
    zipFileExtractor.Extract();
}

static IFileGroupingReporter FindAndReportDuplicatesWithinFolders(
    IFileHasher fileHasher,
    string[] rootFolderPaths,
    IReporter reporter)
{
    return new DuplicatesFileGroupingReporter(
        new FileFinder(
            new FolderFinder(
                rootFolderPaths,
                "*",
                true),
            new ExtensionFileExcluder([".ini"])),
        new DirectoryNameFileGrouper(),
        fileHasher,
        reporter);
}

static IFileGroupingReporter FindAndReportSimilarFilesInIndividualFolders(
    IFileHasher fileHasher,
    IImageComparer imageComparer,
    string[] rootFolderPaths,
    IReporter reporter,
    IFilePairer filePairer)
{
    return new SimilarFilesFileGroupingReporter(
        new FileFinder(
            new FolderFinder(
                rootFolderPaths,
                "*",
                true),
            new ExtensionFileExcluder([".ini"])),
        new DirectoryNameFileGrouper(),
        fileHasher,
        imageComparer,
        reporter,
        filePairer);
}

static IFileGroupingReporter FindAndReportSimilarFilesAcrossFolders(
    IFileHasher fileHasher,
    IImageComparer imageComparer,
    string[] rootFolderPaths,
    IReporter reporter,
    IFilePairer filePairer)
{
    return new SimilarFilesFileGroupingReporter(
        new FileFinder(
            new FolderFinder(
                rootFolderPaths,
                "*",
                true),
            new ExtensionFileExcluder([".ini"])),
        NoopFileGrouper.Instance,
        fileHasher,
        imageComparer,
        reporter,
        filePairer);
}

static IFileGroupingReporter FindAndReportDuplicateAcrossFolders(
    IFileHasher fileHasher,
    string[] rootFolderPaths,
    IReporter reporter)
{
    return new DuplicatesFileGroupingReporter(
        new FileFinder(
            new FolderFinder(
                rootFolderPaths,
                "*",
                true),
            new ExtensionFileExcluder([".ini"])),
        NoopFileGrouper.Instance,
        fileHasher,
        reporter);
}
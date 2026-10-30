using DuplicateFinder;
using DuplicateFinder.FileGroupingReporters;
using DuplicateFinder.Files;
using DuplicateFinder.Files.FileExcluders;
using DuplicateFinder.Files.FileFinders;
using DuplicateFinder.Files.FileGroupers;
using DuplicateFinder.Folders;
using DuplicateFinder.Folders.FolderFinders;
using TextCopy;

const string rootFolder = @"G:\";

var commandType = GetCommandType();

var cachingSha256FileHasher = new CachingSHA256FileHasher();
IFileGroupingReporter? reporter = null;

using (var imageComparer = new ImageMagickImageComparer(cachingSha256FileHasher, 1e-4))
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
                [Path.Combine(rootFolder, @"Other computers\Office PC")]);
            break;

        case CommandType.FindSimilarFilesWithinFolders:
            // Handle finding similar files in individual folders
            reporter = FindAndReportSimilarFilesInIndividualFolders(
                cachingSha256FileHasher,
                imageComparer,
                [Path.Combine(rootFolder, @"G:\Other computers\Office PC\Photos\4 st leonards close")]);
            break;

        case CommandType.FindDuplicatesAcrossFolders:
            // Handle finding duplicates across folder structures
            reporter = FindAndReportDuplicateAcrossFolders(
                cachingSha256FileHasher,
                [Path.Combine(rootFolder, @"Other computers\Office PC")]);
            break;

        case CommandType.FindSimilarFilesAcrossFolders:
            // Handle finding similar files across folder structures
            reporter = FindAndReportSimilarFilesAcrossFolders(
                cachingSha256FileHasher,
                imageComparer,
                [Path.Combine(rootFolder, @"Other computers\Office PC")]);
            break;

        default:
            throw new ArgumentOutOfRangeException();
    }
}

var report = reporter?.Report();
if (report == null)
{
    Console.WriteLine("ERROR! NO REPORT");
    return 1;
}

ClipboardService.SetText(report);
Console.WriteLine(report);

return 0;

static CommandType GetCommandType()
{
    return CommandType.FindSimilarFilesWithinFolders;
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
    string[] rootFolderPaths)
{
    return new DuplicatesFileGroupingReporter(
        new FileFinder(
            new FolderFinder(
                rootFolderPaths,
                "*",
                true),
            new ExtensionFileExcluder([".ini"])),
        new DirectoryNameFileGrouper(),
        fileHasher);
}

static IFileGroupingReporter FindAndReportSimilarFilesInIndividualFolders(
    IFileHasher fileHasher,
    IImageComparer imageComparer,
    string[] rootFolderPaths)
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
        imageComparer);
}

static IFileGroupingReporter FindAndReportSimilarFilesAcrossFolders(
    IFileHasher fileHasher,
    IImageComparer imageComparer,
    string[] rootFolderPaths)
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
        imageComparer);
}

static IFileGroupingReporter FindAndReportDuplicateAcrossFolders(
    IFileHasher fileHasher,
    string[] rootFolderPaths)
{
    return new DuplicatesFileGroupingReporter(
        new FileFinder(
            new FolderFinder(
                rootFolderPaths,
                "*",
                true),
            new ExtensionFileExcluder([".ini"])),
        NoopFileGrouper.Instance,
        fileHasher);
}
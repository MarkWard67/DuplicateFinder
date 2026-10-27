using System.Text;
using DuplicateFinder;
using DuplicateFinder.FileComparers;
using DuplicateFinder.Files;
using DuplicateFinder.Files.FileExcluders;
using DuplicateFinder.Files.FileFinders;
using DuplicateFinder.Folders;
using DuplicateFinder.Folders.FolderExcluders;
using DuplicateFinder.Folders.FolderFinders;

using TextCopy;

const string rootFolder = @"G:\";

var cachingSha256FileHasher = new CachingSHA256FileHasher();

var commandType = GetCommandType();

switch (commandType)
{
    case CommandType.ExtractZipFiles:
        ExtractZipFiles("takeout-20261008*.zip");
        break;

    case CommandType.FindDuplicatesWithinFolders:
        // Handle finding duplicates in individual folders
        FindAndReportDuplicateInIndividualFolders(
            cachingSha256FileHasher,
            [Path.Combine(rootFolder, @"Takeout\Drive\Office PC")]);
        break;

    case CommandType.FindSimilarFilesWithinFolders:
        // Handle finding similar files in individual folders
        FindAndReportSimilarFilesInIndividualFolders(
            cachingSha256FileHasher,
            [Path.Combine(rootFolder, @"Other computers\Office PC")]);
        break;

    case CommandType.FindDuplicatesAcrossFolders:
        // Handle finding duplicates across folder structures
        FindAndReportDuplicateAcrossFolders(
            cachingSha256FileHasher,
            [Path.Combine(rootFolder, @"Takeout\Drive\Office PC")]);
        break;

    case CommandType.FindFilesUniqueToOneFolderStructure:
        // Handle finding files unique to one folder structure
        FindFilesUniqueToOneFolderStructure(cachingSha256FileHasher);
        break;

    default:
        throw new ArgumentOutOfRangeException();
}

return;

static CommandType GetCommandType() => CommandType.FindSimilarFilesWithinFolders;

static void ExtractZipFiles(string zipFileSpec)
{
    var zipFileExtractor = new ZipFileExtractor(
        rootFolder,
        KnownFolders.GetPath(KnownFolder.Downloads),
        zipFileSpec);
    zipFileExtractor.ProgressChanged += (s, e) => Console.WriteLine(e.Value);
    zipFileExtractor.Extract();
}

static void FindAndReportDuplicateInIndividualFolders(
    IFileHasher fileHasher,
    string[] rootFolderPaths)
{
    var duplicateFileFinder = new WithinFolderDuplicateFileFinder(
        new FileFinder(
            new FolderFinder(
                rootFolderPaths,
                "*",
                true),
            new ExplicitFileExcluder()),
        fileHasher);
    var duplications = duplicateFileFinder.FindDuplications();

    var sb = new StringBuilder("DUPLICATE FILES IN INDIVIDUAL FOLDERS").AppendLine();
    foreach (var duplication in duplications)
    {
        foreach (var fileInfo in duplication)
        {
            if (fileInfo.Attributes.ToBeDeleted)
            {
                sb.AppendLine($"@ERASE \"{fileInfo.FileInfo.FullName}\"");
            }
            else
            {
                sb.AppendLine($"@ECHO ERASING {duplication.Length - 1} FILES, RETAINING \"{fileInfo.FileInfo.FullName}\"");
            }
        }

        sb.AppendLine();
    }

    var report = sb.ToString();
    ClipboardService.SetText(report);
    Console.WriteLine(report);
}

static void FindAndReportSimilarFilesInIndividualFolders(
    IFileHasher fileHasher,
    string[] rootFolderPaths)
{
    var similarFilesFinder = new WithinFolderSimilarFilesFinder(
        new FileFinder(
            new FolderFinder(
                rootFolderPaths,
                "*",
                true),
            new ExplicitFileExcluder()),
        fileHasher);
    var similarities = similarFilesFinder.FindSimilarFiles();

    var sb = new StringBuilder("SIMILAR FILES IN INDIVIDUAL FOLDERS").AppendLine();
    const string destinationFolderName = "SimilarFiles";
    foreach (var similarity in similarities)
    {
        var directoryFullName = similarity.Item1.FileInfo.DirectoryName;
        var destDirectoryFullName = Path.Combine(directoryFullName!, destinationFolderName);

        sb.AppendLine($"IF NOT EXIST \"{destDirectoryFullName}\" MKDIR \"{destDirectoryFullName}\"");
        sb.AppendLine($"MOVE \"{similarity.Item1.FileInfo.FullName}\" \"{destDirectoryFullName}\"");

        foreach (var fileInfo in similarity.Item2)
        {
            sb.AppendLine($"MOVE \"{fileInfo.FileInfo.FullName}\" \"{destDirectoryFullName}\"");
        }

        sb.AppendLine();
    }

    var report = sb.ToString();
    ClipboardService.SetText(report);
    Console.WriteLine(report);
}

static void FindAndReportDuplicateAcrossFolders(
    IFileHasher fileHasher,
    string[] rootFolderPaths)
{
    var duplicateFileFinder = new AcrossFoldersDuplicateFileFinder(
        new FileFinder(
            new FolderFinder(
                rootFolderPaths,
                "*",
                true),
            new ExplicitFileExcluder()),
        fileHasher);
    var duplications = duplicateFileFinder.FindDuplications();

    var sb = new StringBuilder("DUPLICATE FILES").AppendLine();
    foreach (var duplication in duplications)
    {
        foreach (var fileInfo in duplication.OrderBy(x => x.FileInfo.DirectoryName))
        {
            sb.AppendLine($"\"{fileInfo.FileInfo.FullName}\"");
        }

        sb.AppendLine();
    }

    var report = sb.ToString();
    ClipboardService.SetText(report);
    Console.WriteLine(report);
}

static void FindFilesUniqueToOneFolderStructure(IFileHasher fileHasher)
{
    var binChecker = new GooglePhotosBinChecker(
        new FileFinder(
            new FolderFinder(
                Path.Combine(rootFolder, @"Takeout\Google Photos\Bin"),
                "*",
                true,
                new GooglePhotosAlbumFolderExcluder()
            ),
            new FileExtensionExcluder([".json"])),
        new FileFinder(
            new FolderFinder(
                Path.Combine(rootFolder, @"Takeout\Drive"),
                "*",
                true),
            new ExplicitFileExcluder()),
        fileHasher);
    var problematicPhotosDeletions = binChecker.CheckBin();

    var sb = new StringBuilder("PROBLEMATIC PHOTO DELETIONS").AppendLine();
    foreach (var problematicPhotosDeletion in problematicPhotosDeletions)
    {
        sb.AppendLine(problematicPhotosDeletion.Item1.FileInfo.FullName);
        sb.AppendLine($"\t{problematicPhotosDeletion.Item1.FileInfo.Length}, {problematicPhotosDeletion.Item1.Attributes.FileHash}");
        foreach (var f in problematicPhotosDeletion.Item2)
        {
            sb.AppendLine($"\t{f.FileInfo.FullName} -- {f.FileInfo.Length} {f.Attributes.FileHash}");
        }
    }

    var report = sb.ToString();
    ClipboardService.SetText(report);
    Console.WriteLine(report);
}
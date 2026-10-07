using System.Text;
using DuplicateFinder;
using DuplicateFinder.Files;
using DuplicateFinder.Folders;
using TextCopy;

//////////// UNZIP GOOGLE TAKEOUT ZIP FILES
//////////var zipFileExtractor = new ZipFileExtractor(
//////////    @"C:\Temp",
//////////    KnownFolders.GetPath(KnownFolder.Downloads),
//////////    "takeout-20261006*.zip");
//////////zipFileExtractor.ProgressChanged += (s, e) => Console.WriteLine(e.Value);
//////////zipFileExtractor.Extract();

var cachingSha256FileHasher = new CachingSHA256FileHasher();

/*
// =================================================================
// DUPLICATES IN INDIVIDUAL FOLDERS WITHIN A SINGLE FOLDER STRUCTURE
// =================================================================
*/
//////////var duplicateFileFinder = new IntraFolderDuplicateFileFinder(
//////////    new FileFinder(
//////////        new FolderFinder(
//////////            @"C:\Temp\Takeout\Drive\Office PC",
//////////            "*",
//////////            true),
//////////        new ExplicitFileExcluder()),
//////////    cachingSha256FileHasher);
//////////var duplications = duplicateFileFinder.FindDuplications();

//////////var sb = new StringBuilder("DUPLICATE FILES IN INDIVIDUAL FOLDERS").AppendLine();
//////////foreach (var duplication in duplications)
//////////{
//////////    foreach (var fileInfo in duplication)
//////////    {
//////////        if (fileInfo.Attributes.ToBeDeleted)
//////////        {
//////////            sb.AppendLine($"@ERASE \"{fileInfo.FileInfo.FullName}\"");
//////////        }
//////////        else
//////////        {
//////////            sb.AppendLine($"@ECHO ERASING {duplication.Length - 1} FILES, RETAINING \"{fileInfo.FileInfo.FullName}\"");
//////////        }
//////////    }

//////////    sb.AppendLine();
//////////}

//////////var duplicatesReport = sb.ToString();
//////////ClipboardService.SetText(duplicatesReport);
//////////Console.WriteLine(duplicatesReport);

/*
// =================================================
// DUPLICATES WITHIN THE UNION OF FOLDERS STRUCTURES
// =================================================
*/
var duplicateFileFinder = new DuplicateFileFinder(
    new FileFinder(
        new FolderFinder(
            [
                @"C:\Temp\Takeout\Drive\Office PC",
                @"C:\Users\windo\Downloads\Mobile Devices"
            ],
            "*",
            true),
        new ExplicitFileExcluder()),
    cachingSha256FileHasher);
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

var duplicatesReport = sb.ToString();
ClipboardService.SetText(duplicatesReport);
Console.WriteLine(duplicatesReport);

/*
// ======================================================================
// FILES IN ONE FOLDER STRUCTURE THAT ARE NOT IN ANOTHER FOLDER STRUCTURE
// ======================================================================
*/
//////////var binChecker = new GooglePhotosBinChecker(
//////////    new FileFinder(
//////////        new FolderFinder(
//////////            @"C:\Temp\Takeout\Google Photos\Bin",
//////////            "*",
//////////            true,
//////////                new GooglePhotosAlbumFolderExcluder()
//////////            ),
//////////            new FileExtensionExcluder([".json"])),
//////////    new FileFinder(
//////////        new FolderFinder(
//////////            @"C:\Temp\Takeout\Drive",
//////////            "*",
//////////            true),
//////////        new ExplicitFileExcluder()),
//////////    cachingSha256FileHasher);
//////////var problematicPhotosDeletions = binChecker.CheckBin();

//////////var sb2 = new StringBuilder("PROBLEMATIC PHOTO DELETIONS");
//////////foreach (var problematicPhotosDeletion in problematicPhotosDeletions)
//////////{
//////////    sb2.AppendLine(problematicPhotosDeletion.Item1.FileInfo.FullName);
//////////    foreach (var f in problematicPhotosDeletion.Item2)
//////////    {
//////////        sb2.AppendLine($"\t{f.FileInfo.FullName}");
//////////    }
//////////}

//////////var deletionsReport = sb2.ToString();
//////////ClipboardService.SetText(deletionsReport);
//////////Console.WriteLine(deletionsReport);
using System.Text;
using DuplicateFinder;
using DuplicateFinder.Files;
using DuplicateFinder.Folders;
using TextCopy;

// UNZIP GOOGLE TAKEOUT ZIP FILES
//////////var zipFileExtractor = new ZipFileExtractor(
//////////    @"C:\Temp",
//////////    KnownFolders.GetPath(KnownFolder.Downloads),
//////////    "takeout-20261006*.zip");
//////////zipFileExtractor.ProgressChanged += (s,e) => Console.WriteLine(e.Value);
//////////zipFileExtractor.Extract();

var cachingSha256FileHasher = new CachingSHA256FileHasher();

// DUPLICATES IN A SINGLE FOLDER STRUCTURE
var duplicateFileFinder = new DuplicateFileFinder(
    new FileFinder(
        new FolderFinder(@"C:\Temp\Takeout\Drive\Office PC", "*", true, ExplicitFolderExcluder.DefaultInstance),
        new ExplicitFileExcluder()),
    cachingSha256FileHasher);
var duplications = duplicateFileFinder.FindDuplications();

var sb = new StringBuilder("DUPLICATE FILES");
foreach (var duplication in duplications)
{
    foreach (var fileInfo in duplication.OrderBy(x => x.DirectoryName)) sb.AppendLine(fileInfo.FullName);

    sb.AppendLine();
}

var duplicatesReport = sb.ToString();
ClipboardService.SetText(duplicatesReport);
Console.WriteLine(duplicatesReport);

// FILES IN ONE FOLDER STRUCTURE THAT ARE NOT IN ANOTHER FOLDER STRUCTURE
//////////var binChecker = new GooglePhotosBinChecker(
//////////    new FileFinder(
//////////        new FolderFinder(@"C:\Temp\Takeout\Google Photos\Bin", "*", true,
//////////            new GooglePhotosAlbumFolderExcluder(ExplicitFolderExcluder.DefaultInstance)),
//////////        new FileExtensionExcluder([".json"], new ExplicitFileExcluder())),
//////////    new FileFinder(
//////////        new FolderFinder(@"C:\Temp\Takeout\Drive", "*", true, ExplicitFolderExcluder.DefaultInstance),
//////////        new ExplicitFileExcluder()),
//////////    cachingSha256FileHasher);
//////////var problematicPhotosDeletions = binChecker.CheckBin();

//////////var sb2 = new StringBuilder("PROBLEMATIC PHOTO DELETIONS");
//////////foreach (var problematicPhotosDeletion in problematicPhotosDeletions)
//////////{
//////////    sb2.AppendLine(problematicPhotosDeletion.Item1.FullName);
//////////    foreach (var f in problematicPhotosDeletion.Item2)
//////////    {
//////////        sb2.AppendLine($"\t{f.FullName}");
//////////    }
//////////}

//////////var deletionsReport = sb2.ToString();
//////////ClipboardService.SetText(deletionsReport);
//////////Console.WriteLine(deletionsReport);
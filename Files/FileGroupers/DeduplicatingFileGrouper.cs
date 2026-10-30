//////////using DuplicateFinder.Files.FileGroupings;

//////////namespace DuplicateFinder.Files.FileGroupers;

//////////internal class DeduplicatingFileGrouper : AggregateFileGrouper
//////////{
//////////    public DeduplicatingFileGrouper(IFileHasher fileHasher)
//////////        : base(
//////////            IdenticalFileInfoGroupingFactory.Instance,
//////////            FileLengthFileGrouper.Instance,
//////////            new FileHashFileGrouper(fileHasher))
//////////    {
//////////    }
//////////}
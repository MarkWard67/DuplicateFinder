using DuplicateFinder.Files.FileGroupings;

namespace DuplicateFinder.Reporters;

internal interface IReporter
{
    string Report(IEnumerable<IFileInfoGrouping> fileGroupings);
}
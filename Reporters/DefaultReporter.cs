using System.Text;
using DuplicateFinder.Files.FileGroupings;

namespace DuplicateFinder.Reporters;

internal class DefaultReporter : IReporter
{
    protected DefaultReporter()
    {
    }

    public static DefaultReporter Instance { get; } = new();

    public virtual string Report(IEnumerable<IFileInfoGrouping> fileGroups)
    {
        var sb = new StringBuilder();
        foreach (var fileGroup in fileGroups)
        {
            sb.AppendLine($"{fileGroup}");
        }

        return sb.ToString();
    }
}
namespace DuplicateFinder.Files.FileGroupings;

internal class FileInfoGroupingKey(params IEnumerable<object?> keys)
{
    private readonly object?[] _keys = keys.ToArray();

    private FileInfoGroupingKey Extend(object? key)
    {
        return new FileInfoGroupingKey(_keys.Concat([key]));
    }

    public string Read(char separator = '|')
    {
        return string.Join(separator, _keys.Select(x => $"{x}"));
    }

    public static FileInfoGroupingKey Extend(
        FileInfoGroupingKey? baseKey,
        object? extension)
    {
        return baseKey == null
            ? new FileInfoGroupingKey(extension)
            : baseKey.Extend(extension);
    }

    public override string ToString()
    {
        return Read();
    }
}
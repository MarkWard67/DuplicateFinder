namespace DuplicateFinder;

internal class StringEventArgs(string value) : EventArgs
{
    public string Value { get; } = value;
}
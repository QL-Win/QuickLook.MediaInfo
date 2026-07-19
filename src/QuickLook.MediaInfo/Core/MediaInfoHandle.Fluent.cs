namespace QuickLook.MediaInfo.Core;

/// <summary>
/// Fluent extension methods for <see cref="MediaInfoHandle"/>.
/// </summary>
public sealed partial class MediaInfoHandle
{
    /// <summary>Opens <paramref name="fileName"/> and returns the same instance for chaining.</summary>
    public MediaInfoHandle WithOpen(string fileName)
    {
        _ = Open(fileName);
        return this;
    }

    /// <summary>Closes the current file and returns the same instance for chaining.</summary>
    public MediaInfoHandle WithClose()
    {
        Close();
        return this;
    }

    /// <summary>Sets an option and exposes the return value via <paramref name="result"/>.</summary>
    public MediaInfoHandle WithOption(string option, string value, out string result)
    {
        result = Option(option, value);
        return this;
    }

    /// <summary>Sets an option with an empty value and exposes the return value via <paramref name="result"/>.</summary>
    public MediaInfoHandle WithOption(string option, out string result)
    {
        result = Option(option);
        return this;
    }
}

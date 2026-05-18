namespace QuickLook.MediaInfo.Core;

/// <summary>
/// Fluent extension methods for <see cref="MediaInfoHandle"/>.
/// </summary>
public static class MediaInfoHandleExtension
{
    /// <summary>Opens <paramref name="fileName"/> and returns the same instance for chaining.</summary>
    public static MediaInfoHandle WithOpen(this MediaInfoHandle self, string fileName)
    {
        _ = self.Open(fileName);
        return self;
    }

    /// <summary>Closes the current file and returns the same instance for chaining.</summary>
    public static MediaInfoHandle WithClose(this MediaInfoHandle self)
    {
        self.Close();
        return self;
    }

    /// <summary>Sets an option and exposes the return value via <paramref name="result"/>.</summary>
    public static MediaInfoHandle WithOption(this MediaInfoHandle self, string option, string value, out string result)
    {
        result = self.Option(option, value);
        return self;
    }

    /// <summary>Sets an option with an empty value and exposes the return value via <paramref name="result"/>.</summary>
    public static MediaInfoHandle WithOption(this MediaInfoHandle self, string option, out string result)
    {
        result = self.Option(option);
        return self;
    }
}

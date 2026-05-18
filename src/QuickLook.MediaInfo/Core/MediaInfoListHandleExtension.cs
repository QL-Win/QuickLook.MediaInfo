namespace QuickLook.MediaInfo.Core;

/// <summary>
/// Fluent extension methods for <see cref="MediaInfoListHandle"/>.
/// </summary>
public static class MediaInfoListHandleExtension
{
    /// <summary>Opens <paramref name="fileName"/> and returns the same instance for chaining.</summary>
    public static MediaInfoListHandle WithOpen(this MediaInfoListHandle self, string fileName)
    {
        self.Open(fileName);
        return self;
    }

    /// <summary>Closes all open files and returns the same instance for chaining.</summary>
    public static MediaInfoListHandle WithClose(this MediaInfoListHandle self)
    {
        self.Close();
        return self;
    }

    /// <summary>Sets an option and exposes the return value via <paramref name="result"/>.</summary>
    public static MediaInfoListHandle WithOption(this MediaInfoListHandle self, string option, string value, out string result)
    {
        result = self.Option(option, value);
        return self;
    }

    /// <summary>Sets an option with an empty value and exposes the return value via <paramref name="result"/>.</summary>
    public static MediaInfoListHandle WithOption(this MediaInfoListHandle self, string option, out string result)
    {
        result = self.Option(option);
        return self;
    }
}

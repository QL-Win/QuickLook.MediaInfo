using QuickLook.MediaInfo.Core;

namespace QuickLook.MediaInfo;

/// <summary>
/// Convenient high-level wrapper over <see cref="MediaInfoHandle"/>.
/// Accepts an optional native library directory so the caller controls where
/// MediaInfo.dll / libmediainfo is loaded from.
/// </summary>
/// <example>
/// <code>
/// using var mi = new MediaInfo(Path.Combine(baseDir, Environment.Is64BitProcess ? "x64" : "x86"));
/// mi.Option("Cover_Data", "base64");
/// mi.Open(filePath);
/// string codec = mi.Get(StreamKind.Video, 0, "Format");
/// </code>
/// </example>
public sealed class MediaInfo : IDisposable
{
    private readonly MediaInfoHandle _handle;

    /// <summary>
    /// Initialises a new instance and optionally configures the native library search path.
    /// </summary>
    /// <param name="dllDirectory">
    /// Directory containing the platform-specific MediaInfo native library.
    /// Pass <c>null</c> to rely on the OS default search path (or a previously configured path).
    /// </param>
    public MediaInfo(string? dllDirectory = null)
    {
        if (dllDirectory != null)
            Libs.LoadDll(dllDirectory);

        _handle = new MediaInfoHandle();
    }

    /// <inheritdoc cref="MediaInfoHandle.Handle"/>
    public nint Handle => _handle.Handle;

    /// <summary>Opens a media file by path.</summary>
    public nint Open(string fileName) => _handle.Open(fileName);

    /// <summary>Initialises buffer-based analysis.</summary>
    public nint OpenBufferInit(long fileSize, long fileOffset) =>
        _handle.OpenBufferInit(fileSize, fileOffset);

    /// <summary>Feeds a buffer chunk during buffer-based analysis.</summary>
    public unsafe nint OpenBufferContinue(byte* buffer, int bufferSize) =>
        _handle.OpenBufferContinue(buffer, bufferSize);

    /// <summary>Returns the byte offset the library wants to seek to next, or -1 if none.</summary>
    public long OpenBufferContinueGoToGet() => _handle.OpenBufferContinueGoToGet();

    /// <summary>Finalises buffer-based analysis.</summary>
    public nint OpenBufferFinalize() => _handle.OpenBufferFinalize();

    /// <summary>Closes the currently open file.</summary>
    public void Close() => _handle.Close();

    /// <summary>Returns the formatted inform string for the current file.</summary>
    public string Inform() => _handle.Inform();

    /// <summary>Gets a metadata parameter value by name.</summary>
    public string Get(StreamKind streamKind, int streamNumber, string parameter,
        InfoKind kindOfInfo = InfoKind.Text, InfoKind kindOfSearch = InfoKind.Name) =>
        _handle.Get(streamKind, streamNumber, parameter, kindOfInfo, kindOfSearch);

    /// <summary>Gets a metadata parameter value by index.</summary>
    public string Get(StreamKind streamKind, int streamNumber, int parameter,
        InfoKind kindOfInfo = InfoKind.Text) =>
        _handle.Get(streamKind, streamNumber, parameter, kindOfInfo);

    /// <summary>Sets or queries a MediaInfo option.</summary>
    public string Option(string option, string value = "") => _handle.Option(option, value);

    /// <summary>Returns the internal state code.</summary>
    public int StateGet() => _handle.StateGet();

    /// <summary>Returns the number of streams of the given kind.</summary>
    public int CountGet(StreamKind streamKind, int streamNumber = -1) =>
        _handle.CountGet(streamKind, streamNumber);

    public void Dispose() => _handle.Dispose();
}

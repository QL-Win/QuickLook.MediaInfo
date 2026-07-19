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
public sealed class MediaInfoNative : IDisposable
{
    private readonly MediaInfoHandle _handle;

    /// <summary>
    /// Initialises a new instance and optionally configures the native library search path.
    /// </summary>
    /// <param name="dllDirectory">
    /// Directory containing the platform-specific MediaInfo native library.
    /// Pass <c>null</c> to rely on the OS default search path (or a previously configured path).
    /// </param>
    public MediaInfoNative(string? dllDirectory = null)
    {
        Libs.LoadDll(dllDirectory);

        _handle = new MediaInfoHandle();
    }

    /// <inheritdoc cref="MediaInfoHandle.Handle"/>
    public nint Handle => _handle.Handle;

    // -- Open / Close ---------------------------------------------------------

    /// <summary>Opens a media file by path. Returns a non-zero value on success.</summary>
    public int Open(string fileName) => (int)_handle.Open(fileName);

    /// <summary>Initialises buffer-based analysis.</summary>
    public int OpenBufferInit(long fileSize, long fileOffset) =>
        (int)_handle.OpenBufferInit(fileSize, fileOffset);

    /// <summary>Feeds a buffer chunk (pointer overload).</summary>
    public int OpenBufferContinue(nint buffer, nint bufferSize) =>
        (int)_handle.OpenBufferContinue(buffer, bufferSize);

    /// <summary>Feeds a buffer chunk (unsafe overload).</summary>
    public unsafe int OpenBufferContinue(byte* buffer, int bufferSize) =>
        (int)_handle.OpenBufferContinue(buffer, bufferSize);

    /// <summary>Returns the byte offset the library wants to seek to next, or -1 if none.</summary>
    public long OpenBufferContinueGoToGet() => _handle.OpenBufferContinueGoToGet();

    /// <summary>Finalises buffer-based analysis.</summary>
    public int OpenBufferFinalize() => (int)_handle.OpenBufferFinalize();

    /// <summary>Closes the currently open file.</summary>
    public void Close() => _handle.Close();

    // -- Inform ---------------------------------------------------------------

    /// <summary>Returns the formatted inform string for the current file.</summary>
    public string Inform() => _handle.Inform();

    // -- Get ------------------------------------------------------------------

    /// <summary>Gets a metadata parameter value by name, with explicit kind and search mode.</summary>
    public string Get(StreamKind streamKind, int streamNumber, string parameter,
        InfoKind kindOfInfo, InfoKind kindOfSearch) =>
        _handle.Get(streamKind, streamNumber, parameter, kindOfInfo, kindOfSearch);

    /// <summary>Gets a metadata parameter value by name, using <see cref="InfoKind.Name"/> for search.</summary>
    public string Get(StreamKind streamKind, int streamNumber, string parameter,
        InfoKind kindOfInfo) =>
        _handle.Get(streamKind, streamNumber, parameter, kindOfInfo, InfoKind.Name);

    /// <summary>Gets the text value of a parameter by name.</summary>
    public string Get(StreamKind streamKind, int streamNumber, string parameter) =>
        _handle.Get(streamKind, streamNumber, parameter, InfoKind.Text, InfoKind.Name);

    /// <summary>Gets a metadata parameter value by index, with explicit kind.</summary>
    public string Get(StreamKind streamKind, int streamNumber, int parameter,
        InfoKind kindOfInfo) =>
        _handle.Get(streamKind, streamNumber, parameter, kindOfInfo);

    /// <summary>Gets the text value of a parameter by index.</summary>
    public string Get(StreamKind streamKind, int streamNumber, int parameter) =>
        _handle.Get(streamKind, streamNumber, parameter, InfoKind.Text);

    // -- Option ---------------------------------------------------------------

    /// <summary>Sets or queries a MediaInfo option with an explicit value.</summary>
    public string Option(string option, string value) => _handle.Option(option, value);

    /// <summary>Queries a MediaInfo option (no value).</summary>
    public string Option(string option) => _handle.Option(option, string.Empty);

    // -- State / Count --------------------------------------------------------

    /// <summary>Returns the internal state code.</summary>
    public int StateGet() => _handle.StateGet();

    /// <summary>Returns the number of streams of the given kind in a specific stream position.</summary>
    public int CountGet(StreamKind streamKind, int streamNumber) =>
        _handle.CountGet(streamKind, streamNumber);

    /// <summary>Returns the total number of streams of the given kind.</summary>
    public int CountGet(StreamKind streamKind) => _handle.CountGet(streamKind, -1);

    // -- IDisposable ----------------------------------------------------------

    public void Dispose() => _handle.Dispose();
}

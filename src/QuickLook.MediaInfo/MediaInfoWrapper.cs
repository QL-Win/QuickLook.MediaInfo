using QuickLook.MediaInfo.Builder;
using QuickLook.MediaInfo.Core;
using QuickLook.MediaInfo.Model;
using System.Globalization;

namespace QuickLook.MediaInfo;

/// <summary>
/// High-level wrapper that parses a media file or stream and exposes structured metadata.
/// </summary>
public class MediaInfoWrapper
{
    private const int BufferSize = 64 * 1024;

    private static readonly NumberFormatInfo ProviderNumber = new() { NumberDecimalSeparator = "." };

    public bool Success { get; private set; }
    public string? Version { get; private set; }

    public bool HasVideo => VideoStreams?.Count > 0;
    public bool HasSubtitles => Subtitles?.Count > 0;
    public bool HasChapters => Chapters?.Count > 0;
    public bool IsDvd { get; private set; }
    public bool IsBluRay { get; private set; }

    /// <summary>Total container duration in milliseconds.</summary>
    public double Duration { get; private set; }

    /// <summary>Container file size in bytes.</summary>
    public long Size { get; private set; }

    public IList<VideoStream> VideoStreams { get; private set; } = [];
    public IList<AudioStream> AudioStreams { get; private set; } = [];
    public IList<SubtitleStream> Subtitles { get; private set; } = [];
    public IList<ChapterStream> Chapters { get; private set; } = [];
    public IList<MenuStream> MenuStreams { get; private set; } = [];

    /// <summary>Best (first) video stream, or <c>null</c> if none.</summary>
    public VideoStream? BestVideoStream => VideoStreams.Count > 0 ? VideoStreams[0] : null;

    /// <summary>Best (first) audio stream, or <c>null</c> if none.</summary>
    public AudioStream? BestAudioStream => AudioStreams.Count > 0 ? AudioStreams[0] : null;

    public double Framerate => BestVideoStream?.FrameRate ?? 0;
    public int Width => BestVideoStream?.Width ?? 0;
    public int Height => BestVideoStream?.Height ?? 0;
    public AspectRatio AspectRatio => BestVideoStream?.AspectRatio ?? AspectRatio.FullScreen;
    public VideoCodec VideoCodec => BestVideoStream?.Codec ?? VideoCodec.Undefined;
    public bool IsInterlaced => BestVideoStream?.Interlaced ?? false;
    public string ScanType => IsInterlaced ? "Interlaced" : "Progressive";

    public string VideoResolution => Width switch
    {
        >= 3840 => "4K",
        >= 1920 => "1080",
        >= 1280 => "720",
        >= 720 => "576",
        >= 480 => "480",
        _ => "Unknown",
    };

    public AudioCodec AudioCodec => BestAudioStream?.Codec ?? AudioCodec.Undefined;
    public int AudioRate => BestAudioStream?.SamplingRate ?? 0;
    public int AudioChannels => BestAudioStream?.Channels ?? 0;

    public string AudioChannelsFriendly => AudioChannels switch
    {
        1 => "Mono",
        2 => "Stereo",
        3 => "2.1",
        4 => "4.0",
        5 => "5.0",
        6 => "5.1",
        7 => "6.1",
        8 => "7.1",
        9 => "7.2",
        10 => "7.2.1",
        _ => AudioChannels.ToString(),
    };

    public MediaInfoWrapper(string filePath)
    {
        if (string.IsNullOrEmpty(filePath)) return;
        try
        {
            Size = File.Exists(filePath) ? new FileInfo(filePath).Length : 0;
            ParseFile(filePath);
        }
        catch { /* suppress parse errors */ }
    }

    public MediaInfoWrapper(Stream inputStream)
    {
        Size = inputStream.Length;
        try
        {
            ParseStream(inputStream);
        }
        catch { }
    }

    private void ParseFile(string filePath)
    {
        using var mi = new MediaInfoHandle();
        if (mi.Handle == IntPtr.Zero) return;

        Version = mi.Option(MediaInfoOptions.Info_Version);
        if (mi.Open(filePath) == IntPtr.Zero) return;

        ParseMedia(mi);
        mi.Close();
    }

    private void ParseStream(Stream stream)
    {
        using var mi = new MediaInfoHandle();
        if (mi.Handle == IntPtr.Zero) return;

        Version = mi.Option(MediaInfoOptions.Info_Version);
        if (stream.CanSeek)
            ParseStreamWithSeek(stream, mi);
        else
            ParseStreamWithoutSeek(stream, mi);

        mi.OpenBufferFinalize();
        ParseMedia(mi);
    }

    private unsafe void ParseStreamWithSeek(Stream stream, MediaInfoHandle mi)
    {
        var buffer = new byte[BufferSize];
        mi.OpenBufferInit(stream.Length, 0L);
        fixed (byte* pBuffer = buffer)
        {
            int read;
            bool running;
            do
            {
                read = stream.Read(buffer, 0, BufferSize);
                running = (mi.OpenBufferContinue(pBuffer, read) & 8) != 8;
                if (running)
                {
                    long seek = mi.OpenBufferContinueGoToGet();
                    if (seek != -1)
                    {
                        long pos = stream.Seek(seek, SeekOrigin.Begin);
                        mi.OpenBufferInit(stream.Length, pos);
                    }
                }
            } while (running && read > 0);
        }
    }

    private unsafe void ParseStreamWithoutSeek(Stream stream, MediaInfoHandle mi)
    {
        mi.Option(MediaInfoOptions.File_IsSeekable, "0");
        mi.OpenBufferInit(-1L, 0L);
        var buffer = new byte[BufferSize];
        fixed (byte* pBuffer = buffer)
        {
            int read;
            do
            {
                read = stream.Read(buffer, 0, BufferSize);
            } while (read > 0 && (mi.OpenBufferContinue(pBuffer, read) & 8) != 8);
        }
    }

    private void ParseMedia(MediaInfoHandle mi)
    {
        // Duration
        if (double.TryParse(mi.Get(StreamKind.General, 0, "Duration"),
            NumberStyles.AllowDecimalPoint, ProviderNumber, out double dur))
            Duration = dur;

        // Video streams
        int videoCount = mi.CountGet(StreamKind.Video);
        for (int i = 0; i < videoCount; i++)
            VideoStreams.Add(new VideoStreamBuilder(mi, i, i).Build());

        // Audio streams
        int audioCount = mi.CountGet(StreamKind.Audio);
        for (int i = 0; i < audioCount; i++)
            AudioStreams.Add(new AudioStreamBuilder(mi, i, i).Build());

        // Subtitle streams
        int textCount = mi.CountGet(StreamKind.Text);
        for (int i = 0; i < textCount; i++)
            Subtitles.Add(new SubtitleStreamBuilder(mi, i, i).Build());

        // Chapter streams (Other)
        int otherCount = mi.CountGet(StreamKind.Other);
        for (int i = 0; i < otherCount; i++)
            Chapters.Add(new ChapterStreamBuilder(mi, i, i).Build());

        // Menu streams
        int menuCount = mi.CountGet(StreamKind.Menu);
        for (int i = 0; i < menuCount; i++)
            MenuStreams.Add(new MenuStreamBuilder(mi, i, i).Build());

        Success = true;
    }

    // -- Inform helpers --------------------------------------------------------

    /// <summary>
    /// Opens <paramref name="filePath"/> and returns the raw MediaInfo Inform output
    /// in the requested format and language.
    /// </summary>
    public static string GetInform(
        string filePath,
        MediaInfoOptions.InformFormat format = MediaInfoOptions.InformFormat.Text,
        Language_ISO639? language = null,
        string? customTemplate = null)
    {
        using var mi = new MediaInfoHandle();
        if (mi.Handle == IntPtr.Zero) return "Unable to load MediaInfo library";

        // Set language
        if (language.HasValue)
        {
            string langContent = language.Value.ToIso639();
            if (!string.IsNullOrEmpty(langContent))
                mi.Option(MediaInfoOptions.Language, langContent);
        }

        // Set inform format
        string informValue = format switch
        {
            MediaInfoOptions.InformFormat.XML => "XML",
            MediaInfoOptions.InformFormat.MAXML => "MAXML",
            MediaInfoOptions.InformFormat.JSON => "JSON",
            MediaInfoOptions.InformFormat.HTML => "HTML",
            MediaInfoOptions.InformFormat.CSV => "CSV",
            MediaInfoOptions.InformFormat.Custom => customTemplate ?? string.Empty,
            _ => string.Empty, // Text is the default
        };

        if (!string.IsNullOrEmpty(informValue))
            mi.Option(MediaInfoOptions.Inform, informValue);

        mi.Open(filePath);
        string result = mi.Inform();
        mi.Close();
        return result;
    }

    /// <summary>
    /// Returns the library version string, or <c>null</c> when the DLL is not found.
    /// </summary>
    public static string? GetLibraryVersion()
    {
        try
        {
            using var mi = new MediaInfoHandle();
            if (mi.Handle == IntPtr.Zero) return null;
            return mi.Option(MediaInfoOptions.Info_Version);
        }
        catch { return null; }
    }
}

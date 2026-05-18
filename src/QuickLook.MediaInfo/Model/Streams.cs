using QuickLook.MediaInfo.Core;

namespace QuickLook.MediaInfo.Model;

public class VideoStream : LanguageMediaStream
{
    public override MediaStreamKind Kind => MediaStreamKind.Video;
    protected internal override StreamKind StreamKind => StreamKind.Video;

    public double FrameRate { get; set; }
    public int FrameRateNumerator { get; set; }
    public int FrameRateDenominator { get; set; }
    public FrameRateMode FrameRateMode { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public double Bitrate { get; set; }
    public AspectRatio AspectRatio { get; set; }
    public bool Interlaced { get; set; }
    public StereoMode Stereoscopic { get; set; }
    public string Format { get; set; } = default!;
    public VideoCodec Codec { get; set; }
    public string CodecProfile { get; set; } = default!;
    public VideoStandard Standard { get; set; }
    public ColorSpace ColorSpace { get; set; }
    public TransferCharacteristic TransferCharacteristics { get; set; }
    public ChromaSubSampling SubSampling { get; set; }
    public TimeSpan Duration { get; set; }
    public string TimeCodeFirstFrame { get; set; } = default!;
    public string TimeCodeLastFrame { get; set; } = default!;
    public bool TimeCodeDropFrame { get; set; }
    public Hdr Hdr { get; set; }
    public int BitDepth { get; set; }
}

public class AudioStream : LanguageMediaStream
{
    public override MediaStreamKind Kind => MediaStreamKind.Audio;
    protected internal override StreamKind StreamKind => StreamKind.Audio;

    public AudioCodec Codec { get; set; }
    public TimeSpan Duration { get; set; }
    public double Bitrate { get; set; }
    public BitrateMode BitrateMode { get; set; }
    public double FrameRate { get; set; }
    public int FrameRateNumerator { get; set; }
    public int FrameRateDenominator { get; set; }
    public int SamplingRate { get; set; }
    public int Channels { get; set; }
    public int BitDepth { get; set; }
    public string TimeCodeFirstFrame { get; set; } = default!;
    public string TimeCodeLastFrame { get; set; } = default!;
    public bool TimeCodeDropFrame { get; set; }
    public string TimeCodeSettings { get; set; } = default!;
}

public class SubtitleStream : LanguageMediaStream
{
    public override MediaStreamKind Kind => MediaStreamKind.Text;
    protected internal override StreamKind StreamKind => StreamKind.Text;

    public string Format { get; set; } = default!;
    public SubtitleCodec Codec { get; set; }
    public TimeSpan Duration { get; set; }
    public double FrameRate { get; set; }
    public int FrameRateNumerator { get; set; }
    public int FrameRateDenominator { get; set; }
    public string TimeCodeFirstFrame { get; set; } = default!;
    public string TimeCodeLastFrame { get; set; } = default!;
    public bool TimeCodeDropFrame { get; set; }
    public string TimeCodeSettings { get; set; } = default!;
    public string TimeCodeSource { get; set; } = default!;
}

public class ChapterStream : MediaStream
{
    public override MediaStreamKind Kind => MediaStreamKind.Menu;
    protected internal override StreamKind StreamKind => StreamKind.Other;

    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}

public class MenuStream : LanguageMediaStream
{
    public override MediaStreamKind Kind => MediaStreamKind.Menu;
    protected internal override StreamKind StreamKind => StreamKind.Menu;

    public TimeSpan Duration { get; set; }
}

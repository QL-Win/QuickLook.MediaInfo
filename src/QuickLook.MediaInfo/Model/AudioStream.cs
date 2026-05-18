using QuickLook.MediaInfo.Core;

namespace QuickLook.MediaInfo.Model;

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

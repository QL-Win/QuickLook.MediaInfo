using QuickLook.MediaInfo.Core;

namespace QuickLook.MediaInfo.Model;

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

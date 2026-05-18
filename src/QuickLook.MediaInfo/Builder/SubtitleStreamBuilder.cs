using QuickLook.MediaInfo.Core;
using QuickLook.MediaInfo.Model;

namespace QuickLook.MediaInfo.Builder;

internal class SubtitleStreamBuilder(MediaInfoHandle info, int number, int position)
    : LanguageMediaStreamBuilder<SubtitleStream>(info, number, position)
{
    public override MediaStreamKind Kind => MediaStreamKind.Text;
    protected override StreamKind StreamKind => StreamKind.Text;

    private static readonly Dictionary<string, SubtitleCodec> Codecs = new(StringComparer.OrdinalIgnoreCase)
    {
        { "S_TEXT/UTF8", SubtitleCodec.Utf8 }, { "UTF-8", SubtitleCodec.Utf8 },
        { "S_TEXT/ASS", SubtitleCodec.Ass }, { "ASS", SubtitleCodec.Ass },
        { "S_TEXT/SSA", SubtitleCodec.Ssa }, { "SSA", SubtitleCodec.Ssa },
        { "S_TEXT/USF", SubtitleCodec.Usf },
        { "S_VOBSUB", SubtitleCodec.Vobsub }, { "VobSub", SubtitleCodec.Vobsub },
        { "S_HDMV/PGS", SubtitleCodec.Pgs }, { "PGS", SubtitleCodec.Pgs },
        { "S_HDMV/TEXTST", SubtitleCodec.EButtd },
        { "dvb_subtitle", SubtitleCodec.Dvb }, { "DVB Subtitle", SubtitleCodec.Dvb },
        { "Teletext", SubtitleCodec.Teletext }, { "SRT", SubtitleCodec.Srt },
        { "SCTE-27", SubtitleCodec.Scte27 },
        { "Timed Text", SubtitleCodec.SmpteTimedText }, { "S_TEXT/WEBVTT", SubtitleCodec.Srt },
        { "KATE", SubtitleCodec.Kate },
        { "Closed captions", SubtitleCodec.ClosedCaptions },
    };

    public override SubtitleStream Build()
    {
        var result = base.Build();
        result.Format = Get("Format");
        result.Codec = DetectCodec();
        result.Duration = Get<TimeSpan>("Duration", TagBuilderHelper.TryGetTimeSpan);
        result.FrameRate = Get<double>("FrameRate", TagBuilderHelper.TryGetDouble);
        result.FrameRateNumerator = Get<int>("FrameRate_Num", TagBuilderHelper.TryGetInt);
        result.FrameRateDenominator = Get<int>("FrameRate_Den", TagBuilderHelper.TryGetInt);
        result.TimeCodeFirstFrame = Get("TimeCode_FirstFrame");
        result.TimeCodeLastFrame = Get("TimeCode_LastFrame");
        result.TimeCodeDropFrame = Get("TimeCode_DropFrame").TryGetBool(out bool df) && df;
        result.TimeCodeSettings = Get("TimeCode_Settings");
        result.TimeCodeSource = Get("TimeCode_Source");
        return result;
    }

    private SubtitleCodec DetectCodec()
    {
        var codecId = Get("CodecID");
        if (Codecs.TryGetValue(codecId, out var codec)) return codec;
        var format = Get("Format");
        if (Codecs.TryGetValue(format, out codec)) return codec;
        return SubtitleCodec.Undefined;
    }
}

using QuickLook.MediaInfo.Core;
using QuickLook.MediaInfo.Model;

namespace QuickLook.MediaInfo.Builder;

internal class VideoStreamBuilder(MediaInfoHandle info, int number, int position)
    : LanguageMediaStreamBuilder<VideoStream>(info, number, position)
{
    public override MediaStreamKind Kind => MediaStreamKind.Video;
    protected override StreamKind StreamKind => StreamKind.Video;

    private static readonly Dictionary<string, VideoCodec> VideoCodecs = new(StringComparer.OrdinalIgnoreCase)
    {
        { "V_UNCOMPRESSED", VideoCodec.Uncompressed },
        { "V_DIRAC", VideoCodec.Dirac }, { "DIRAC", VideoCodec.Dirac },
        { "MPEG-4 Visual", VideoCodec.Mpeg4 },
        { "V_MPEG4/IS0/AVC", VideoCodec.Mpeg4Is0Avc }, { "V_MPEG4/ISO/AVC", VideoCodec.Mpeg4IsoAvc },
        { "V_MPEGH/ISO/HEVC", VideoCodec.MpeghIsoHevc },
        { "AVC", VideoCodec.Mpeg4IsoAvc }, { "AVC1", VideoCodec.Mpeg4IsoAvc },
        { "H264", VideoCodec.Mpeg4IsoAvc }, { "H.264", VideoCodec.Mpeg4IsoAvc },
        { "V_MPEG1", VideoCodec.Mpeg1 }, { "MPEG-1", VideoCodec.Mpeg1 }, { "MPEG1", VideoCodec.Mpeg1 },
        { "V_MPEG2", VideoCodec.Mpeg2 }, { "MPEG-2", VideoCodec.Mpeg2 }, { "MPEG2", VideoCodec.Mpeg2 },
        { "MPEG Video", VideoCodec.Mpeg2 },
        { "V_PRORES", VideoCodec.ProRes }, { "PRORES", VideoCodec.ProRes },
        { "V_REAL/RV10", VideoCodec.RealRv10 }, { "V_REAL/RV20", VideoCodec.RealRv20 },
        { "V_REAL/RV30", VideoCodec.RealRv30 }, { "V_REAL/RV40", VideoCodec.RealRv40 },
        { "V_THEORA", VideoCodec.Theora },
        { "V_VP6", VideoCodec.Vp6 }, { "VP6", VideoCodec.Vp6 },
        { "V_VP7", VideoCodec.Vp7 }, { "VP7", VideoCodec.Vp7 },
        { "V_VP8", VideoCodec.Vp8 }, { "VP8", VideoCodec.Vp8 },
        { "V_VP9", VideoCodec.Vp9 }, { "VP9", VideoCodec.Vp9 },
        { "V_AV1", VideoCodec.Av1 }, { "AV1", VideoCodec.Av1 },
        { "VC1", VideoCodec.Vc1 }, { "VC-1", VideoCodec.Vc1 }, { "WVC1", VideoCodec.Vc1 },
        { "SPRK", VideoCodec.Sprk }, { "SVQ1", VideoCodec.Svq1 }, { "SVQ2", VideoCodec.Svq2 }, { "SVQ3", VideoCodec.Svq3 },
        { "WMV1", VideoCodec.Wmv1 }, { "WMV2", VideoCodec.Wmv2 }, { "WMV3", VideoCodec.Wmv3 },
        { "HAP", VideoCodec.Hap },
    };

    private static readonly Dictionary<string, AspectRatio> Ratios = new(StringComparer.Ordinal)
    {
        { "1:1", AspectRatio.Opaque }, { "5:4", AspectRatio.HighEndDataGraphics },
        { "4:3", AspectRatio.FullScreen }, { "1.334", AspectRatio.FullScreen },
        { "3:2", AspectRatio.DigitalSlrCameras }, { "1.5", AspectRatio.DigitalSlrCameras },
        { "16:9", AspectRatio.HighDefinitionTv }, { "1.778", AspectRatio.HighDefinitionTv },
        { "16:10", AspectRatio.WideScreenDisplay }, { "1.6", AspectRatio.WideScreenDisplay },
        { "1.85", AspectRatio.WideScreen },
        { "21:9", AspectRatio.CinemaScope }, { "2.334", AspectRatio.CinemaScope },
    };

    private static readonly Dictionary<string, ChromaSubSampling> ChromaSubSamplings = new(StringComparer.OrdinalIgnoreCase)
    {
        { "4:2:0", ChromaSubSampling.Sampling420 }, { "4:2:2", ChromaSubSampling.Sampling422 },
        { "4:4:4", ChromaSubSampling.Sampling444 }, { "4:4:4:4", ChromaSubSampling.Sampling4444 },
        { "4:1:1", ChromaSubSampling.Sampling411 }, { "4:1:0", ChromaSubSampling.Sampling410 },
        { "3:3:2", ChromaSubSampling.Sampling332 }, { "5:5:5", ChromaSubSampling.Sampling555 },
        { "5:6:5", ChromaSubSampling.Sampling565 }, { "8:8:8", ChromaSubSampling.Sampling888 },
    };

    private static readonly Dictionary<string, Hdr> HdrFormats = new(StringComparer.OrdinalIgnoreCase)
    {
        { "Dolby Vision", Hdr.DolbyVision },
        { "HDR10", Hdr.HDR10 }, { "PQ", Hdr.HDR10 }, { "SMPTE ST 2086", Hdr.HDR10 },
        { "HDR10 Plus", Hdr.HDR10Plus }, { "HDR10+", Hdr.HDR10Plus },
        { "HLG", Hdr.HLG },
        { "SL-HDR", Hdr.SLHDR1 },
        { "HDR Vivid", Hdr.HdrVivid }, { "T/UWA 005 (HDR Vivid)", Hdr.HdrVivid },
    };

    private static readonly Dictionary<string, StereoMode> StereoModes = new(StringComparer.OrdinalIgnoreCase)
    {
        { "side-by-side (left eye first)", StereoMode.SideBySideLeft },
        { "side-by-side (right eye first)", StereoMode.SideBySideRight },
        { "top-bottom (left eye first)", StereoMode.TopBottomLeft },
        { "top-bottom (right eye first)", StereoMode.TopBottomRight },
        { "checkerboard (left eye first)", StereoMode.CheckerboardLeft },
        { "checkerboard (right eye first)", StereoMode.CheckerboardRight },
        { "row interleaved (left eye first)", StereoMode.RowInterleavedLeft },
        { "row interleaved (right eye first)", StereoMode.RowInterleavedRight },
        { "column interleaved (left eye first)", StereoMode.ColumnInterleavedLeft },
        { "column interleaved (right eye first)", StereoMode.ColumnInterleavedRight },
        { "anaglyph (cyan/red)", StereoMode.AnaglyphCyanRed },
        { "anaglyph (green/magenta)", StereoMode.AnaglyphGreenMagenta },
        { "both eyes laced in one block (left eye first)", StereoMode.BothEyesLacedLeft },
        { "both eyes laced in one block (right eye first)", StereoMode.BothEyesLacedRight },
    };

    public override VideoStream Build()
    {
        var result = base.Build();

        result.Format = Get("Format");
        result.Codec = DetectCodec();
        result.CodecProfile = Get("Format_Profile");
        result.Duration = Get<TimeSpan>("Duration", TagBuilderHelper.TryGetTimeSpan);
        result.Width = Get<int>("Width", TagBuilderHelper.TryGetInt);
        result.Height = Get<int>("Height", TagBuilderHelper.TryGetInt);
        result.Bitrate = Get<double>("BitRate", TagBuilderHelper.TryGetDouble);
        result.FrameRate = Get<double>("FrameRate", TagBuilderHelper.TryGetDouble);
        result.FrameRateNumerator = Get<int>("FrameRate_Num", TagBuilderHelper.TryGetInt);
        result.FrameRateDenominator = Get<int>("FrameRate_Den", TagBuilderHelper.TryGetInt);
        result.FrameRateMode = Get("FrameRate_Mode") is "CFR" ? FrameRateMode.Cfr : Get("FrameRate_Mode") is "VFR" ? FrameRateMode.Vfr : FrameRateMode.Undefined;
        result.Interlaced = Get("ScanType").Equals("Interlaced", StringComparison.OrdinalIgnoreCase);
        result.BitDepth = Get<int>("BitDepth", TagBuilderHelper.TryGetInt);
        result.TimeCodeFirstFrame = Get("TimeCode_FirstFrame");
        result.TimeCodeLastFrame = Get("TimeCode_LastFrame");
        result.TimeCodeDropFrame = Get("TimeCode_DropFrame").TryGetBool(out bool df) && df;

        var ratioStr = Get("DisplayAspectRatio");
        result.AspectRatio = Ratios.TryGetValue(ratioStr, out var ratio) ? ratio : AspectRatio.FullScreen;

        var stereoStr = Get("MultiView_Layout");
        result.Stereoscopic = StereoModes.TryGetValue(stereoStr, out var stereo) ? stereo : StereoMode.Mono;

        var chromaStr = Get("ChromaSubsampling");
        result.SubSampling = ChromaSubSamplings.TryGetValue(chromaStr, out var chroma) ? chroma : ChromaSubSampling.Undefined;

        var hdrStr = Get("HDR_Format");
        if (!HdrFormats.TryGetValue(hdrStr, out var hdr))
        {
            var transferStr = Get("transfer_characteristics");
            HdrFormats.TryGetValue(transferStr, out hdr);
        }
        result.Hdr = hdr;

        return result;
    }

    private VideoCodec DetectCodec()
    {
        var codecId = Get("CodecID");
        if (VideoCodecs.TryGetValue(codecId, out var codec)) return codec;

        var format = Get("Format");
        if (VideoCodecs.TryGetValue(format, out codec)) return codec;

        return VideoCodec.Undefined;
    }
}

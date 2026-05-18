using QuickLook.MediaInfo.Core;
using QuickLook.MediaInfo.Model;

namespace QuickLook.MediaInfo.Builder;

internal class AudioStreamBuilder(MediaInfoHandle info, int number, int position)
    : LanguageMediaStreamBuilder<AudioStream>(info, number, position)
{
    public override MediaStreamKind Kind => MediaStreamKind.Audio;
    protected override StreamKind StreamKind => StreamKind.Audio;

    private static readonly Dictionary<string, AudioCodec> CodecIds = new(StringComparer.OrdinalIgnoreCase)
    {
        { "A_MPEG/L1", AudioCodec.MpegLayer1 }, { "A_MPEG/L2", AudioCodec.MpegLayer2 },
        { "A_MPEG/L3", AudioCodec.MpegLayer3 },
        { "A_PCM/INT/BIG", AudioCodec.PcmIntBig }, { "A_PCM/INT/LIT", AudioCodec.PcmIntLit },
        { "A_PCM/FLOAT/IEEE", AudioCodec.PcmFloatIeee },
        { "A_AC3", AudioCodec.Ac3 }, { "A_AC3/BSID9", AudioCodec.Ac3Bsid9 }, { "A_AC3/BSID10", AudioCodec.Ac3Bsid10 },
        { "A_DTS", AudioCodec.Dts }, { "A_DTS-HD", AudioCodec.DtsHd }, { "A_EAC3", AudioCodec.Eac3 },
        { "A_FLAC", AudioCodec.Flac }, { "A_OPUS", AudioCodec.Opus }, { "A_TTA1", AudioCodec.Tta1 },
        { "A_VORBIS", AudioCodec.Vorbis }, { "A_WAVPACK4", AudioCodec.WavPack4 }, { "A_WAVPACK", AudioCodec.WavPack },
        { "A_REAL/14_4", AudioCodec.Real14_4 }, { "A_REAL/28_8", AudioCodec.Real28_8 },
        { "A_REAL/COOK", AudioCodec.RealCook }, { "A_REAL/SIPR", AudioCodec.RealSipr },
        { "A_REAL/RALF", AudioCodec.RealRalf }, { "A_REAL/ATRC", AudioCodec.RealAtrc },
        { "A_TRUEHD", AudioCodec.Truehd }, { "A_MLP", AudioCodec.Mlp },
        { "A_AAC", AudioCodec.Aac }, { "A_ALAC", AudioCodec.Alac }, { "A_APE", AudioCodec.Ape },
        { "SAMR", AudioCodec.Amr },
    };

    private static readonly Dictionary<string, AudioCodec> Codecs = new(StringComparer.OrdinalIgnoreCase)
    {
        { "MPA1L1", AudioCodec.MpegLayer1 }, { "MPA1L2", AudioCodec.MpegLayer2 }, { "MPA1L3", AudioCodec.MpegLayer3 },
        { "MPEG Audio", AudioCodec.MpegLayer3 },
        { "PCM", AudioCodec.PcmIntLit }, { "PCM BIG", AudioCodec.PcmIntBig }, { "PCM LITTLE", AudioCodec.PcmIntLit },
        { "PCM/FLOAT/IEEE", AudioCodec.PcmFloatIeee },
        { "AC3", AudioCodec.Ac3 }, { "AC-3", AudioCodec.Ac3 }, { "AC3+", AudioCodec.Eac3 },
        { "E-AC-3", AudioCodec.Eac3 }, { "EAC3", AudioCodec.Eac3 }, { "EAC-3", AudioCodec.Eac3 },
        { "AC-3+ATMOS", AudioCodec.Ac3Atmos }, { "E-AC-3+ATMOS", AudioCodec.Eac3Atmos },
        { "DTS", AudioCodec.Dts }, { "DTS-HD", AudioCodec.DtsHd }, { "DTS-UHD", AudioCodec.DtsUhd },
        { "FLAC", AudioCodec.Flac }, { "OPUS", AudioCodec.Opus },
        { "TTA", AudioCodec.Tta1 }, { "TTA1", AudioCodec.Tta1 },
        { "VORBIS", AudioCodec.Vorbis },
        { "WAVPACK", AudioCodec.WavPack }, { "WAVPACK4", AudioCodec.WavPack4 },
        { "WAVE", AudioCodec.Wave }, { "WAVE64", AudioCodec.Wave64 },
        { "TRUEHD", AudioCodec.Truehd }, { "TRUEHD / AC3", AudioCodec.Truehd },
        { "TRUEHD+ATMOS", AudioCodec.TruehdAtmos }, { "TRUEHD+ATMOS / TRUEHD", AudioCodec.TruehdAtmos },
        { "MLP", AudioCodec.Mlp }, { "MLP FBA", AudioCodec.Truehd },
        { "AAC", AudioCodec.Aac }, { "ALAC", AudioCodec.Alac }, { "APE", AudioCodec.Ape },
        { "AMR", AudioCodec.Amr }, { "WMAPRO", AudioCodec.WmaPro },
        { "Ac4", AudioCodec.Ac4 }, { "Ac-4", AudioCodec.Ac4 },
        { "APAC", AudioCodec.Apac }, { "APT-X100", AudioCodec.Aptx100 },
        { "Auro-Cx", AudioCodec.AuroCx }, { "Dolby E", AudioCodec.DolbyE }, { "Dolby ED2", AudioCodec.DolbyEd2 },
        { "Monkey's Audio", AudioCodec.Ape }, { "DSD", AudioCodec.Dsd },
        { "Atrac3", AudioCodec.Atrac3 }, { "Atrac1", AudioCodec.Atrac1 }, { "Atrac9", AudioCodec.Atrac9 },
        { "MPEG-H 3D Audio", AudioCodec.Mpeg3DAudio }, { "USAC", AudioCodec.Usac },
    };

    private static readonly Dictionary<string, AudioCodec> AdditionalFeatures = new(StringComparer.OrdinalIgnoreCase)
    {
        { "MLP FBA 16-ch", AudioCodec.TruehdAtmos }, { "FBA 16-ch", AudioCodec.TruehdAtmos },
        { "16-ch", AudioCodec.TruehdAtmos }, { "AC-3 16-ch", AudioCodec.TruehdAtmos },
        { "JOC", AudioCodec.Eac3Atmos }, { "Dep JOC", AudioCodec.Eac3Atmos },
        { "XLL", AudioCodec.DtsHdMa }, { "ES XXCH XLL", AudioCodec.DtsHdMa }, { "ES XLL", AudioCodec.DtsHdMa },
        { "XLL X", AudioCodec.DtsX }, { "XXCH XLL X", AudioCodec.DtsX }, { "ES XXCH XLL X", AudioCodec.DtsX },
        { "XBR", AudioCodec.DtsHdHra }, { "ES XXCH XBR", AudioCodec.DtsHdHra },
    };

    private static readonly Dictionary<string, BitrateMode> BitrateModes = new(StringComparer.OrdinalIgnoreCase)
    {
        { "CBR", BitrateMode.Cbr }, { "VBR", BitrateMode.Vbr }, { "CQ", BitrateMode.Cq },
    };

    public override AudioStream Build()
    {
        var result = base.Build();
        result.Codec = DetectCodec();
        result.Duration = Get<TimeSpan>("Duration", TagBuilderHelper.TryGetTimeSpan);
        result.Bitrate = Get<double>("BitRate", TagBuilderHelper.TryGetDouble);
        result.SamplingRate = Get<int>("SamplingRate", TagBuilderHelper.TryGetInt);
        result.Channels = Get<int>("Channel(s)", TagBuilderHelper.TryGetInt);
        result.BitDepth = Get<int>("BitDepth", TagBuilderHelper.TryGetInt);
        result.FrameRate = Get<double>("FrameRate", TagBuilderHelper.TryGetDouble);
        result.FrameRateNumerator = Get<int>("FrameRate_Num", TagBuilderHelper.TryGetInt);
        result.FrameRateDenominator = Get<int>("FrameRate_Den", TagBuilderHelper.TryGetInt);
        result.TimeCodeFirstFrame = Get("TimeCode_FirstFrame");
        result.TimeCodeLastFrame = Get("TimeCode_LastFrame");
        result.TimeCodeDropFrame = Get("TimeCode_DropFrame").TryGetBool(out bool df) && df;
        result.TimeCodeSettings = Get("TimeCode_Settings");

        var bitrateMode = Get("BitRate_Mode");
        result.BitrateMode = BitrateModes.TryGetValue(bitrateMode, out var bm) ? bm : BitrateMode.Undefined;
        return result;
    }

    private AudioCodec DetectCodec()
    {
        var codecId = Get("CodecID");
        if (CodecIds.TryGetValue(codecId, out var codec)) return codec;

        var format = Get("Format");
        var addFeatures = Get("Format_AdditionalFeatures");

        if (!string.IsNullOrEmpty(addFeatures) && AdditionalFeatures.TryGetValue(addFeatures, out codec))
            return codec;

        if (Codecs.TryGetValue(format, out codec)) return codec;

        return AudioCodec.Undefined;
    }
}

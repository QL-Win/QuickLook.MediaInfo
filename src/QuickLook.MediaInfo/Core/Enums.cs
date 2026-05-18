using System.ComponentModel;

namespace QuickLook.MediaInfo.Core;

public enum StreamKind
{
    General,
    Video,
    Audio,
    Text,
    Other,
    Image,
    Menu,
}

public enum InfoKind
{
    Name,
    Text,
    Measure,
    Options,
    NameText,
    MeasureText,
    Info,
    HowTo,
}

public enum InfoOptions
{
    ShowInInform,
    Support,
    ShowInSupported,
    TypeOfValue,
}

[Flags]
public enum InfoFileOptions
{
    FileOption_Nothing = 0x00,
    FileOption_NoRecursive = 0x01,
    FileOption_CloseAll = 0x02,
    FileOption_Max = 0x04,
}

[Flags]
public enum Status
{
    None = 0x00,
    Accepted = 0x01,
    Filled = 0x02,
    Updated = 0x04,
    Finalized = 0x08,
}

public static class MediaInfoOptions
{
    public const string Complete = "Complete";
    public const string Complete_Get = "Complete_Get";
    public const string BlockMethod = "BlockMethod";
    public const string BlockMethod_Get = "BlockMethod_Get";
    public const string Internet = "Internet";
    public const string Internet_Get = "Internet_Get";
    public const string MultipleValues = "MultipleValues";
    public const string MultipleValues_Get = "MultipleValues_Get";
    public const string ReadByHuman = "ReadByHuman";
    public const string ReadByHuman_Get = "ReadByHuman_Get";
    public const string Legacy = "Legacy";
    public const string Legacy_Get = "Legacy_Get";
    public const string LegacyStreamDisplay = "LegacyStreamDisplay";
    public const string LegacyStreamDisplay_Get = "LegacyStreamDisplay_Get";
    public const string SkipBinaryData = "SkipBinaryData";
    public const string SkipBinaryData_Get = "SkipBinaryData_Get";
    public const string ParseSpeed = "ParseSpeed";
    public const string ParseSpeed_Get = "ParseSpeed_Get";
    public const string Verbosity = "Verbosity";
    public const string Verbosity_Get = "Verbosity_Get";
    public const string Compat = "Compat";
    public const string Compat_Get = "Compat_Get";
    public const string Https = "Https";
    public const string Https_Get = "Https_Get";
    public const string TraceFormat = "Trace_Format";
    public const string TraceFormat_Get = "Trace_Format_Get";
    public const string Demux = "Demux";
    public const string Demux_Get = "Demux_Get";
    public const string LineSeparator = "LineSeparator";
    public const string LineSeparator_Get = "LineSeparator_Get";
    public const string Version = "Version";
    public const string Version_Get = "Version_Get";
    public const string ColumnSeparator = "ColumnSeparator";
    public const string ColumnSeparator_Get = "ColumnSeparator_Get";
    public const string TagSeparator = "TagSeparator";
    public const string TagSeparator_Get = "TagSeparator_Get";
    public const string Quote = "Quote";
    public const string Quote_Get = "Quote_Get";
    public const string DecimalPoint = "DecimalPoint";
    public const string DecimalPoint_Get = "DecimalPoint_Get";
    public const string ThousandsPoint = "ThousandsPoint";
    public const string ThousandsPoint_Get = "ThousandsPoint_Get";
    public const string CarriageReturnReplace = "CarriageReturnReplace";
    public const string CarriageReturnReplace_Get = "CarriageReturnReplace_Get";
    public const string StreamMax = "StreamMax";
    public const string StreamMax_Get = "StreamMax_Get";

    /// <summary>
    /// Sets the output language. Pass an ISO 639 language code (e.g. "en", "zh-CN") or
    /// the full CSV content returned by <see cref="LanguageExtension.ToIso639"/>.
    /// </summary>
    public const string Language = "Language";

    public const string Language_Get = "Language_Get";

    public const string Inform = "Inform";
    public const string Inform_Get = "Inform_Get";
    public const string Inform_Version = "Inform_Version";
    public const string Inform_Version_Get = "Inform_Version_Get";
    public const string Inform_Timestamp = "Inform_Timestamp";
    public const string Inform_Timestamp_Get = "Inform_Timestamp_Get";

    public const string Format_Get = "Format_Get";
    public const string Codec_Get = "Codec_Get";
    public const string AcquisitionDataOutputMode = "AcquisitionDataOutputMode";
    public const string AcquisitionDataOutputMode_Get = "AcquisitionDataOutputMode_Get";

    /// <summary>Info_Version option — returns the library version string.</summary>
    public const string Info_Version = "Info_Version";

    /// <summary>File_IsSeekable option — controls whether the input stream supports seeking.</summary>
    public const string File_IsSeekable = "File_IsSeekable";

    public static class Returns
    {
        public const string Frame = "Frame";
        public const string Container = "Container";
        public const string Elementary = "Elementary";
        public const string OptionNotKnown = "Option not known";
    }

    public enum Trace_Format
    {
        Trace_Format_Tree,
        Trace_Format_CSV,
        Trace_Format_XML,
        Trace_Format_MICRO_XML,
    }

    /// <summary>
    /// Inform output format keywords supported by MediaInfo natively.
    /// Pass these to Option("Inform", value) before calling Inform().
    /// </summary>
    public enum InformFormat
    {
        /// <summary>Default human-readable text tree view.</summary>
        Text,

        /// <summary>XML output.</summary>
        XML,

        /// <summary>MediaArea XML (MAXML) output.</summary>
        MAXML,

        /// <summary>JSON output.</summary>
        JSON,

        /// <summary>HTML output.</summary>
        HTML,

        /// <summary>CSV output.</summary>
        CSV,

        /// <summary>Custom inform template; pass your own template string.</summary>
        Custom,
    }
}

/// <summary>
/// ISO 639 language codes supported by the MediaInfo language system.
/// <c>ar;be;bg;ca;cs;da;de;en;es;eu;fa;fr;gl;gr;hu;id;it;ja;ko;lt;nl;pl;pt;pt-BR;ro;ru;sk;sq;sv;th;tr;uk;zh-CN;zh-HK;zh-TW;hr;hy;ka</c>
/// </summary>
public enum Language_ISO639
{
    [Description("en")] English,
    [Description("fr")] French,
    [Description("de")] German,
    [Description("es")] Spanish,
    [Description("it")] Italian,
    [Description("ja")] Japanese,
    [Description("ko")] Korean,
    [Description("zh-CN")] ChineseSimplified,
    [Description("zh-HK")] ChineseHongKong,
    [Description("zh-TW")] ChineseTraditional,
    [Description("ru")] Russian,
    [Description("pt")] Portuguese,
    [Description("pt-BR")] PortugueseBrazilian,
    [Description("nl")] Dutch,
    [Description("sv")] Swedish,
    [Description("da")] Danish,
    [Description("pl")] Polish,
    [Description("cs")] Czech,
    [Description("sk")] Slovak,
    [Description("hu")] Hungarian,
    [Description("bg")] Bulgarian,
    [Description("uk")] Ukrainian,
    [Description("ro")] Romanian,
    [Description("hr")] Croatian,
    [Description("lt")] Lithuanian,
    [Description("gr")] Greek,
    [Description("tr")] Turkish,
    [Description("fa")] Persian,
    [Description("ar")] Arabic,
    [Description("id")] Indonesian,
    [Description("th")] Thai,
    [Description("hy")] Armenian,
    [Description("ka")] Georgian,
    [Description("be")] Belarusian,
    [Description("ca")] Catalan,
    [Description("eu")] Basque,
    [Description("gl")] Galician,
    [Description("sq")] Albanian,
}

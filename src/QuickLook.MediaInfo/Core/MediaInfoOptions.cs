namespace QuickLook.MediaInfo.Core;

public static class MediaInfoOptions
{
    public const string Complete                        = "Complete";
    public const string Complete_Get                   = "Complete_Get";
    public const string BlockMethod                    = "BlockMethod";
    public const string BlockMethod_Get                = "BlockMethod_Get";
    public const string Internet                       = "Internet";
    public const string Internet_Get                   = "Internet_Get";
    public const string MultipleValues                 = "MultipleValues";
    public const string MultipleValues_Get             = "MultipleValues_Get";
    public const string ReadByHuman                    = "ReadByHuman";
    public const string ReadByHuman_Get                = "ReadByHuman_Get";
    public const string Legacy                         = "Legacy";
    public const string Legacy_Get                     = "Legacy_Get";
    public const string LegacyStreamDisplay            = "LegacyStreamDisplay";
    public const string LegacyStreamDisplay_Get        = "LegacyStreamDisplay_Get";
    public const string SkipBinaryData                 = "SkipBinaryData";
    public const string SkipBinaryData_Get             = "SkipBinaryData_Get";
    public const string ParseSpeed                     = "ParseSpeed";
    public const string ParseSpeed_Get                 = "ParseSpeed_Get";
    public const string Verbosity                      = "Verbosity";
    public const string Verbosity_Get                  = "Verbosity_Get";
    public const string Compat                         = "Compat";
    public const string Compat_Get                     = "Compat_Get";
    public const string Https                          = "Https";
    public const string Https_Get                      = "Https_Get";
    public const string TraceFormat                    = "Trace_Format";
    public const string TraceFormat_Get                = "Trace_Format_Get";
    public const string Demux                          = "Demux";
    public const string Demux_Get                      = "Demux_Get";
    public const string LineSeparator                  = "LineSeparator";
    public const string LineSeparator_Get              = "LineSeparator_Get";
    public const string Version                        = "Version";
    public const string Version_Get                    = "Version_Get";
    public const string ColumnSeparator                = "ColumnSeparator";
    public const string ColumnSeparator_Get            = "ColumnSeparator_Get";
    public const string TagSeparator                   = "TagSeparator";
    public const string TagSeparator_Get               = "TagSeparator_Get";
    public const string Quote                          = "Quote";
    public const string Quote_Get                      = "Quote_Get";
    public const string DecimalPoint                   = "DecimalPoint";
    public const string DecimalPoint_Get               = "DecimalPoint_Get";
    public const string ThousandsPoint                 = "ThousandsPoint";
    public const string ThousandsPoint_Get             = "ThousandsPoint_Get";
    public const string CarriageReturnReplace          = "CarriageReturnReplace";
    public const string CarriageReturnReplace_Get      = "CarriageReturnReplace_Get";
    public const string StreamMax                      = "StreamMax";
    public const string StreamMax_Get                  = "StreamMax_Get";

    /// <summary>
    /// Sets the output language. Pass an ISO 639 language code (e.g. "en", "zh-CN") or
    /// the full CSV content returned by <see cref="LanguageExtension.ToIso639"/>.
    /// </summary>
    public const string Language                       = "Language";
    public const string Language_Get                   = "Language_Get";

    public const string Inform                         = "Inform";
    public const string Inform_Get                     = "Inform_Get";
    public const string Inform_Version                 = "Inform_Version";
    public const string Inform_Version_Get             = "Inform_Version_Get";
    public const string Inform_Timestamp               = "Inform_Timestamp";
    public const string Inform_Timestamp_Get           = "Inform_Timestamp_Get";

    public const string Format_Get                     = "Format_Get";
    public const string Codec_Get                      = "Codec_Get";
    public const string AcquisitionDataOutputMode      = "AcquisitionDataOutputMode";
    public const string AcquisitionDataOutputMode_Get  = "AcquisitionDataOutputMode_Get";

    /// <summary>Info_Version option — returns the library version string.</summary>
    public const string Info_Version                   = "Info_Version";

    /// <summary>File_IsSeekable option — controls whether the input stream supports seeking.</summary>
    public const string File_IsSeekable                = "File_IsSeekable";

    public static class Returns
    {
        public const string Frame          = "Frame";
        public const string Container      = "Container";
        public const string Elementary     = "Elementary";
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
        /// <summary>Default human-readable text tree view (same as Tree).</summary>
        Text,

        /// <summary>Tree view — equivalent to the default text output.</summary>
        Tree,

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

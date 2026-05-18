using QuickLook.MediaInfo;
using QuickLook.MediaInfo.Core;

// -----------------------------------------------------------------------------
// QuickLook.MediaInfo.ConsoleTest
//
// Usage:
//   consoletest --file <path> [--format <text|xml|json|csv|html|maxml|custom>]
//               [--language <en|zh-CN|ja|fr|...>]
//               [--template <inform-template-string>]
//               [--structured]
//               [--dll-dir <directory>]
//               [--version]
//               [--help]
//
// Formats supported natively by MediaInfo:
//   text   - human-readable text tree (default)
//   xml    - XML
//   json   - JSON
//   csv    - CSV
//   html   - HTML
//   maxml  - MediaArea XML (MAXML)
//   custom - custom Inform template (provide via --template)
//
// Language codes supported (ISO 639 / IETF):
//   en, zh-CN, zh-HK, zh-TW, ja, ko, fr, de, es, it, ru, pt, pt-BR,
//   nl, sv, da, pl, cs, sk, hu, bg, uk, ro, hr, lt, gr, tr, fa, ar,
//   id, th, hy, ka, be, ca, eu, gl, sq  (etc.)
// -----------------------------------------------------------------------------

string? filePath = null;
string? formatArg = null;
string? languageArg = null;
string? customTemplate = null;
string? dllDir = null;
bool structured = false;
bool showVersion = false;
bool showHelp = false;

// Parse arguments
for (int i = 0; i < args.Length; i++)
{
    string arg = args[i];
    switch (arg.ToLowerInvariant())
    {
        case "--file" or "-f":
            filePath = NextArg(args, ref i);
            break;

        case "--format":
            formatArg = NextArg(args, ref i);
            break;

        case "--language" or "--lang" or "-l":
            languageArg = NextArg(args, ref i);
            break;

        case "--template" or "-t":
            customTemplate = NextArg(args, ref i);
            break;

        case "--dll-dir":
            dllDir = NextArg(args, ref i);
            break;

        case "--structured" or "-s":
            structured = true;
            break;

        case "--version" or "-v":
            showVersion = true;
            break;

        case "--help" or "-h" or "-?":
            showHelp = true;
            break;

        default:
            // Treat bare argument as file path
            if (!arg.StartsWith('-') && filePath is null)
                filePath = arg;
            break;
    }
}

if (showHelp || (filePath is null && !showVersion))
{
    PrintHelp();
    return;
}

// Optional: load MediaInfo.dll from custom directory
if (!string.IsNullOrEmpty(dllDir))
{
    try { Libs.LoadDll(dllDir); }
    catch (Exception ex) { Console.Error.WriteLine($"Warning: failed to load DLL from '{dllDir}': {ex.Message}"); }
}

if (showVersion)
{
    string? ver = MediaInfoWrapper.GetLibraryVersion();
    Console.WriteLine(ver ?? "MediaInfo library not found.");
    return;
}

if (!File.Exists(filePath))
{
    Console.Error.WriteLine($"Error: file not found: {filePath}");
    Environment.Exit(1);
    return;
}

// Resolve language
Language_ISO639? language = ResolveLanguage(languageArg);

// Resolve format
var format = ResolveFormat(formatArg, out string? effectiveCustomTemplate);
if (format == MediaInfoOptions.InformFormat.Custom && customTemplate is null)
    effectiveCustomTemplate = null; // will produce empty output — warn user

if (format == MediaInfoOptions.InformFormat.Custom && string.IsNullOrEmpty(effectiveCustomTemplate))
{
    Console.Error.WriteLine("Error: --format custom requires --template <template>.");
    Environment.Exit(1);
    return;
}

if (structured)
{
    // High-level structured output
    var wrapper = new MediaInfoWrapper(filePath!);
    PrintStructured(wrapper);
}
else
{
    // Raw Inform output
    string output = MediaInfoWrapper.GetInform(
        filePath!,
        format,
        language,
        effectiveCustomTemplate ?? customTemplate);

    Console.OutputEncoding = System.Text.Encoding.UTF8;
    Console.WriteLine(output);
}

// -----------------------------------------------------------------------------
// Helpers
// -----------------------------------------------------------------------------

static string? NextArg(string[] args, ref int i)
{
    if (i + 1 < args.Length) return args[++i];
    return null;
}

static MediaInfoOptions.InformFormat ResolveFormat(string? formatArg, out string? customTemplate)
{
    customTemplate = null;
    return formatArg?.ToLowerInvariant() switch
    {
        "xml" => MediaInfoOptions.InformFormat.XML,
        "json" => MediaInfoOptions.InformFormat.JSON,
        "csv" => MediaInfoOptions.InformFormat.CSV,
        "html" => MediaInfoOptions.InformFormat.HTML,
        "maxml" => MediaInfoOptions.InformFormat.MAXML,
        "custom" => MediaInfoOptions.InformFormat.Custom,
        _ => MediaInfoOptions.InformFormat.Text,
    };
}

static Language_ISO639? ResolveLanguage(string? code)
{
    if (string.IsNullOrEmpty(code)) return null;

    // Match by description attribute
    foreach (Language_ISO639 lang in Enum.GetValues<Language_ISO639>())
    {
        if (lang.ToCode().Equals(code, StringComparison.OrdinalIgnoreCase))
            return lang;
    }

    // Match by enum name
    if (Enum.TryParse<Language_ISO639>(code, ignoreCase: true, out var byName))
        return byName;

    Console.Error.WriteLine($"Warning: unknown language code '{code}', using default (English).");
    return null;
}

static void PrintStructured(MediaInfoWrapper w)
{
    Console.OutputEncoding = System.Text.Encoding.UTF8;
    Console.WriteLine($"Library version  : {w.Version ?? "n/a"}");
    Console.WriteLine($"Parse success    : {w.Success}");
    Console.WriteLine($"Duration         : {TimeSpan.FromMilliseconds(w.Duration):hh\\:mm\\:ss\\.fff}");
    Console.WriteLine($"File size        : {w.Size:N0} bytes");
    Console.WriteLine($"Has video        : {w.HasVideo}");
    Console.WriteLine($"Has subtitles    : {w.HasSubtitles}");
    Console.WriteLine($"Has chapters     : {w.HasChapters}");
    Console.WriteLine($"Is DVD           : {w.IsDvd}");
    Console.WriteLine($"Is Blu-Ray       : {w.IsBluRay}");
    Console.WriteLine();

    if (w.HasVideo)
    {
        Console.WriteLine("--- Video ---------------------------------------");
        foreach (var v in w.VideoStreams)
        {
            Console.WriteLine($"  [{v.StreamPosition}] {v.Format}  {v.Width}x{v.Height}  {v.FrameRate:F3} fps  {v.Bitrate / 1000:F0} kbps");
            Console.WriteLine($"      Codec={v.Codec}  Profile={v.CodecProfile}  Interlaced={v.Interlaced}  HDR={v.Hdr}  BitDepth={v.BitDepth}");
            Console.WriteLine($"      Duration={v.Duration:hh\\:mm\\:ss}  AspectRatio={v.AspectRatio}");
        }
        Console.WriteLine();
    }

    if ((w.AudioStreams?.Count ?? 0) > 0)
    {
        Console.WriteLine("--- Audio ---------------------------------------");
        foreach (var a in w.AudioStreams!)
        {
            Console.WriteLine($"  [{a.StreamPosition}] {a.Codec}  {a.SamplingRate} Hz  {a.Channels} ch  {a.BitDepth} bit  {a.Bitrate / 1000:F0} kbps");
            Console.WriteLine($"      Language={a.Language}  Duration={a.Duration:hh\\:mm\\:ss}");
        }
        Console.WriteLine();
    }

    if (w.HasSubtitles)
    {
        Console.WriteLine("--- Subtitles -----------------------------------");
        foreach (var s in w.Subtitles!)
            Console.WriteLine($"  [{s.StreamPosition}] {s.Codec}  Language={s.Language}  Default={s.Default}  Forced={s.Forced}");
        Console.WriteLine();
    }

    if (w.HasChapters)
    {
        Console.WriteLine("--- Chapters ------------------------------------");
        foreach (var c in w.Chapters!)
            Console.WriteLine($"  [{c.StreamPosition}] {c.Name}  Start={c.StartTime:hh\\:mm\\:ss}  End={c.EndTime:hh\\:mm\\:ss}");
        Console.WriteLine();
    }
}

static void PrintHelp()
{
    Console.WriteLine("""
        QuickLook.MediaInfo.ConsoleTest — MediaInfo output tool

        Usage:
          consoletest --file <path> [options]
          consoletest <path>               (shorthand)

        Options:
          --file,   -f <path>    Path to the media file (required)
          --format     <name>    Output format (default: text)
                                   text   Human-readable text (default)
                                   xml    XML
                                   json   JSON
                                   csv    CSV
                                   html   HTML
                                   maxml  MediaArea XML
                                   custom Custom template (use with --template)
          --template, -t <tpl>  Custom Inform template string (for --format custom)
          --language, -l <code> Output language ISO 639 code, e.g.:
                                   en, zh-CN, zh-HK, zh-TW, ja, ko, fr, de, ru, ...
          --structured, -s      Print structured property summary instead of raw Inform
          --dll-dir <dir>       Directory containing MediaInfo.dll
          --version,  -v        Print MediaInfo library version and exit
          --help,     -h        Show this help

        Examples:
          consoletest --file sample.mkv
          consoletest --file sample.mkv --format xml --language zh-CN
          consoletest --file sample.mkv --format json
          consoletest --file sample.mkv --format custom --template "Video;%Format%/%Width%x%Height%"
          consoletest --file sample.mkv --structured --language ja
        """);
}

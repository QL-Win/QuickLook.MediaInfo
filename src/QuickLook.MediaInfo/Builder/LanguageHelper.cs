namespace QuickLook.MediaInfo.Builder;

/// <summary>
/// Utilities for mapping ISO-639 short codes to language names and Windows LCIDs.
/// </summary>
public static class LanguageHelper
{
    private static readonly Dictionary<string, int> Lcids = new(StringComparer.OrdinalIgnoreCase)
    {
        { "ARA", 0x0401 }, { "AR", 0x0401 },
        { "BUL", 0x0402 }, { "BG", 0x0402 },
        { "CAT", 0x0403 }, { "CA", 0x0403 },
        { "ZHO", 0x0404 }, { "ZH", 0x0404 },
        { "CES", 0x0405 }, { "CS", 0x0405 },
        { "DAN", 0x0406 }, { "DA", 0x0406 },
        { "DEU", 0x0407 }, { "DE", 0x0407 },
        { "ELL", 0x0408 }, { "EL", 0x0408 },
        { "ENG", 0x0409 }, { "EN", 0x0409 },
        { "SPA", 0x040A }, { "ES", 0x040A },
        { "FIN", 0x040B }, { "FI", 0x040B },
        { "FRA", 0x040C }, { "FR", 0x040C },
        { "HEB", 0x040D }, { "HE", 0x040D },
        { "HUN", 0x040E }, { "HU", 0x040E },
        { "ITA", 0x0410 }, { "IT", 0x0410 },
        { "JPN", 0x0411 }, { "JA", 0x0411 },
        { "KOR", 0x0412 }, { "KO", 0x0412 },
        { "NLD", 0x0413 }, { "NL", 0x0413 },
        { "NOB", 0x0414 }, { "NO", 0x0414 }, { "NB", 0x0414 },
        { "POL", 0x0415 }, { "PL", 0x0415 },
        { "POR", 0x0416 }, { "PT", 0x0416 },
        { "RON", 0x0418 }, { "RO", 0x0418 },
        { "RUS", 0x0419 }, { "RU", 0x0419 },
        { "HRV", 0x041A }, { "HR", 0x041A },
        { "SLK", 0x041B }, { "SK", 0x041B },
        { "SWE", 0x041D }, { "SV", 0x041D },
        { "THA", 0x041E }, { "TH", 0x041E },
        { "TUR", 0x041F }, { "TR", 0x041F },
        { "UKR", 0x0422 }, { "UK", 0x0422 },
        { "LIT", 0x0427 }, { "LT", 0x0427 },
        { "LAV", 0x0426 }, { "LV", 0x0426 },
        { "EST", 0x0425 }, { "ET", 0x0425 },
        { "FAS", 0x0429 }, { "FA", 0x0429 },
        { "IND", 0x0421 }, { "ID", 0x0421 },
        { "HYE", 0x042B }, { "HY", 0x042B },
        { "KAT", 0x0437 }, { "KA", 0x0437 },
        { "BEL", 0x0423 }, { "BE", 0x0423 },
        { "SLV", 0x0424 }, { "SL", 0x0424 },
        { "MKD", 0x042F }, { "MK", 0x042F },
        { "ALB", 0x041C }, { "SQ", 0x041C },
        { "GLG", 0x0456 }, { "GL", 0x0456 },
        { "EUS", 0x042D }, { "EU", 0x042D },
        { "CAL", 0x0403 },
    };

    private static readonly Dictionary<string, string> LcidToName = new()
    {
        { "0401", "Arabic" }, { "0402", "Bulgarian" }, { "0403", "Catalan" },
        { "0404", "Chinese" }, { "0405", "Czech" }, { "0406", "Danish" },
        { "0407", "German" }, { "0408", "Greek" }, { "0409", "English" },
        { "040a", "Spanish" }, { "040b", "Finnish" }, { "040c", "French" },
        { "040d", "Hebrew" }, { "040e", "Hungarian" }, { "0410", "Italian" },
        { "0411", "Japanese" }, { "0412", "Korean" }, { "0413", "Dutch" },
        { "0414", "Norwegian" }, { "0415", "Polish" }, { "0416", "Portuguese" },
        { "0418", "Romanian" }, { "0419", "Russian" }, { "041a", "Croatian" },
        { "041b", "Slovak" }, { "041d", "Swedish" }, { "041e", "Thai" },
        { "041f", "Turkish" }, { "0421", "Indonesian" }, { "0422", "Ukrainian" },
        { "0423", "Belarusian" }, { "0424", "Slovenian" }, { "0425", "Estonian" },
        { "0426", "Latvian" }, { "0427", "Lithuanian" }, { "0429", "Persian" },
        { "042b", "Armenian" }, { "042c", "Azeri" }, { "042d", "Basque" },
        { "042f", "Macedonian" }, { "0436", "Afrikaans" }, { "0437", "Georgian" },
        { "041c", "Albanian" }, { "0456", "Galician" },
    };

    public static int GetLcidByShortName(string shortName) =>
        Lcids.TryGetValue(shortName ?? string.Empty, out int lcid) ? lcid : 0;

    public static string GetLanguageByShortName(string shortName)
    {
        if (string.IsNullOrEmpty(shortName)) return string.Empty;
        int lcid = GetLcidByShortName(shortName);
        if (lcid == 0) return shortName;
        string key = lcid.ToString("x").PadLeft(4, '0');
        return LcidToName.TryGetValue(key, out string? name) ? name : shortName;
    }
}

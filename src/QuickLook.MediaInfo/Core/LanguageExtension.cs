using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace QuickLook.MediaInfo.Core;

/// <summary>
/// Extension methods for <see cref="MediaInfoLanguage"/> and <see cref="CultureInfo"/>
/// that produce the language parameter value expected by MediaInfo's <c>Language</c> option.
/// </summary>
public static class LanguageExtension
{
    private static readonly string[] SupportedCodes =
        "ar;be;bg;ca;cs;da;de;en;es;eu;fa;fr;gl;gr;hu;id;it;ja;ko;lt;nl;pl;pt;pt-BR;ro;ru;sk;sq;sv;th;tr;uk;zh-CN;zh-HK;zh-TW;hr;hy;ka"
        .Split(';');

    /// <summary>
    /// Converts a <see cref="MediaInfoLanguage"/> value to its ISO 639 code string (e.g. "zh-CN").
    /// </summary>
    public static string ToCode(this MediaInfoLanguage language)
    {
        return typeof(MediaInfoLanguage)
            .GetField(language.ToString())
            ?.GetCustomAttribute<DescriptionAttribute>()
            ?.Description ?? "en";
    }

    /// <summary>
    /// Converts a <see cref="MediaInfoLanguage"/> to a <see cref="CultureInfo"/>.
    /// </summary>
    public static CultureInfo ToCultureInfo(this MediaInfoLanguage language)
    {
        string code = language.ToCode();
        try
        {
            return CultureInfo.GetCultureInfo(code);
        }
        catch
        {
            return CultureInfo.GetCultureInfo("en");
        }
    }

    /// <summary>
    /// Converts a <see cref="MediaInfoLanguage"/> to the ISO 639 CSV content
    /// that can be passed directly to <c>Option("Language", value)</c>.
    /// </summary>
    public static string ToIso639(this MediaInfoLanguage language)
        => language.ToCultureInfo().ToIso639();

    /// <summary>
    /// Converts a <see cref="CultureInfo"/> to the ISO 639 CSV content
    /// that can be passed directly to <c>Option("Language", value)</c>.
    /// </summary>
    public static string ToIso639(this CultureInfo culture, Encoding? encoding = null)
    {
        string normalizedCode = NormalizeCode(culture);
        string resourceName = $"QuickLook.MediaInfo.Resource.Plugin.Language.{normalizedCode}.csv";
        using Stream? stream = typeof(LanguageExtension).Assembly.GetManifestResourceStream(resourceName);
        if (stream is null)
            return string.Empty;

        using StreamReader reader = new(stream, encoding ?? Encoding.UTF8);
        return reader.ReadToEnd();
    }

    private static string NormalizeCode(CultureInfo culture)
    {
        string name = culture.Name switch
        {
            "zh" => "zh-CN",
            "zh-CN" or "zh-HK" or "zh-TW" or "pt-BR" => culture.Name,
            _ => culture.TwoLetterISOLanguageName,
        };

        return Array.IndexOf(SupportedCodes, name) >= 0
            ? name
            : "en";
    }
}

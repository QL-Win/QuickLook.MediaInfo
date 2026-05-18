using System.ComponentModel;

namespace QuickLook.MediaInfo.Core;

/// <summary>
/// ISO 639 language codes supported by the MediaInfo language system.
/// <c>ar;be;bg;ca;cs;da;de;en;es;eu;fa;fr;gl;gr;hu;id;it;ja;ko;lt;nl;pl;pt;pt-BR;ro;ru;sk;sq;sv;th;tr;uk;zh-CN;zh-HK;zh-TW;hr;hy;ka</c>
/// </summary>
public enum MediaInfoLanguage
{
    [Description("en")]    English,
    [Description("fr")]    French,
    [Description("de")]    German,
    [Description("es")]    Spanish,
    [Description("it")]    Italian,
    [Description("ja")]    Japanese,
    [Description("ko")]    Korean,
    [Description("zh-CN")] ChineseSimplified,
    [Description("zh-HK")] ChineseHongKong,
    [Description("zh-TW")] ChineseTraditional,
    [Description("ru")]    Russian,
    [Description("pt")]    Portuguese,
    [Description("pt-BR")] PortugueseBrazilian,
    [Description("nl")]    Dutch,
    [Description("sv")]    Swedish,
    [Description("da")]    Danish,
    [Description("pl")]    Polish,
    [Description("cs")]    Czech,
    [Description("sk")]    Slovak,
    [Description("hu")]    Hungarian,
    [Description("bg")]    Bulgarian,
    [Description("uk")]    Ukrainian,
    [Description("ro")]    Romanian,
    [Description("hr")]    Croatian,
    [Description("lt")]    Lithuanian,
    [Description("gr")]    Greek,
    [Description("tr")]    Turkish,
    [Description("fa")]    Persian,
    [Description("ar")]    Arabic,
    [Description("id")]    Indonesian,
    [Description("th")]    Thai,
    [Description("hy")]    Armenian,
    [Description("ka")]    Georgian,
    [Description("be")]    Belarusian,
    [Description("ca")]    Catalan,
    [Description("eu")]    Basque,
    [Description("gl")]    Galician,
    [Description("sq")]    Albanian,
}

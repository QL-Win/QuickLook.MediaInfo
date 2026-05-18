using QuickLook.MediaInfo.Core;
using QuickLook.MediaInfo.Model;

namespace QuickLook.MediaInfo.Builder;

internal abstract class LanguageMediaStreamBuilder<TStream>(MediaInfoHandle info, int number, int position)
    : MediaStreamBuilder<TStream>(info, number, position)
    where TStream : LanguageMediaStream, new()
{
    public override TStream Build()
    {
        var result = base.Build();
        var lang = Get("Language").ToLower();
        result.Language = LanguageHelper.GetLanguageByShortName(lang);
        result.LanguageIetf = Get("LanguageIETF");
        result.Default = Get<bool>("Default", TagBuilderHelper.TryGetBool);
        result.Forced = Get<bool>("Forced", TagBuilderHelper.TryGetBool);
        result.Lcid = LanguageHelper.GetLcidByShortName(lang);
        result.StreamSize = Get<long>("StreamSize", TagBuilderHelper.TryGetLong);
        return result;
    }
}

using QuickLook.MediaInfo.Core;
using QuickLook.MediaInfo.Model;

namespace QuickLook.MediaInfo.Builder;

internal class MenuStreamBuilder(MediaInfoHandle info, int number, int position)
    : LanguageMediaStreamBuilder<MenuStream>(info, number, position)
{
    public override MediaStreamKind Kind => MediaStreamKind.Menu;
    protected override StreamKind StreamKind => StreamKind.Menu;

    public override MenuStream Build()
    {
        var result = base.Build();
        result.Duration = Get<TimeSpan>("Duration", TagBuilderHelper.TryGetTimeSpan);
        return result;
    }
}

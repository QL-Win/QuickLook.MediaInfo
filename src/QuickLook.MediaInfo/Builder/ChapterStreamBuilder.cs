using QuickLook.MediaInfo.Core;
using QuickLook.MediaInfo.Model;

namespace QuickLook.MediaInfo.Builder;

internal class ChapterStreamBuilder(MediaInfoHandle info, int number, int position)
    : MediaStreamBuilder<ChapterStream>(info, number, position)
{
    public override MediaStreamKind Kind => MediaStreamKind.Menu;
    protected override StreamKind StreamKind => StreamKind.Other;

    public override ChapterStream Build()
    {
        var result = base.Build();
        result.StartTime = Get<TimeSpan>("Duration_Start", TagBuilderHelper.TryGetTimeSpan);
        result.EndTime = Get<TimeSpan>("Duration_End", TagBuilderHelper.TryGetTimeSpan);
        return result;
    }
}

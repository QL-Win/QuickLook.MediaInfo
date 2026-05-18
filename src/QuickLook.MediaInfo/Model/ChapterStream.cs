using QuickLook.MediaInfo.Core;

namespace QuickLook.MediaInfo.Model;

public class ChapterStream : MediaStream
{
    public override MediaStreamKind Kind => MediaStreamKind.Menu;
    protected internal override StreamKind StreamKind => StreamKind.Other;

    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}

using QuickLook.MediaInfo.Core;

namespace QuickLook.MediaInfo.Model;

public class MenuStream : LanguageMediaStream
{
    public override MediaStreamKind Kind => MediaStreamKind.Menu;
    protected internal override StreamKind StreamKind => StreamKind.Menu;

    public TimeSpan Duration { get; set; }
}

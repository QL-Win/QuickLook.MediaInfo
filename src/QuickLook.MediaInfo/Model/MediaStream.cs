using QuickLook.MediaInfo.Core;

namespace QuickLook.MediaInfo.Model;

public abstract class MediaStream
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public int StreamPosition { get; set; }
    public int StreamNumber { get; set; }
    public abstract MediaStreamKind Kind { get; }
    protected internal abstract StreamKind StreamKind { get; }
}

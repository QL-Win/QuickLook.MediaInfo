using QuickLook.MediaInfo.Core;

namespace QuickLook.MediaInfo.Model;

public enum MediaStreamKind
{
    Video,
    Audio,
    Text,
    Image,
    Menu,
}

public abstract class MediaStream
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public int StreamPosition { get; set; }
    public int StreamNumber { get; set; }
    public abstract MediaStreamKind Kind { get; }
    protected internal abstract StreamKind StreamKind { get; }
}

public abstract class LanguageMediaStream : MediaStream
{
    public string Language { get; set; } = default!;
    public string LanguageIetf { get; set; } = default!;
    public int Lcid { get; set; }
    public bool Default { get; set; }
    public bool Forced { get; set; }
    public long StreamSize { get; set; }
}

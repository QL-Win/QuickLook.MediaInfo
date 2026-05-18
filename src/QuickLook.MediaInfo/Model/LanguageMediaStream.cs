namespace QuickLook.MediaInfo.Model;

public abstract class LanguageMediaStream : MediaStream
{
    public string Language { get; set; } = default!;
    public string LanguageIetf { get; set; } = default!;
    public int Lcid { get; set; }
    public bool Default { get; set; }
    public bool Forced { get; set; }
    public long StreamSize { get; set; }
}

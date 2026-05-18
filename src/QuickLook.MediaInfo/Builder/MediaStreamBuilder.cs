using QuickLook.MediaInfo.Core;
using QuickLook.MediaInfo.Model;

namespace QuickLook.MediaInfo.Builder;

internal delegate bool ParseDelegate<T>(string source, out T result);

internal abstract class MediaStreamBuilder<TStream>(MediaInfoHandle info, int number, int position)
    where TStream : MediaStream, new()
{
    protected MediaInfoHandle Info { get; } = info;
    protected int StreamNumber { get; } = number;
    protected int StreamPosition { get; } = position;
    public abstract MediaStreamKind Kind { get; }
    protected abstract StreamKind StreamKind { get; }

    public virtual TStream Build()
    {
        var idStr = Get("ID");
        if (!idStr.TryGetInt(out int id))
        {
            var parts = idStr.Split('/');
            if (parts.Length >= 1) parts[0].Trim().TryGetInt(out id);
        }

        return new TStream
        {
            Id = id,
            Name = Get("Title"),
            StreamPosition = StreamPosition,
            StreamNumber = StreamNumber,
        };
    }

    protected string Get(string parameter, Func<string?, string>? transform = null)
    {
        var raw = Info.Get(StreamKind, StreamPosition, parameter);
        return transform is null ? (raw ?? string.Empty) : (transform(raw) ?? string.Empty);
    }

    protected T? Get<T>(string parameter, ParseDelegate<T> convert, Func<string?, string>? transform = null)
    {
        var raw = Get(parameter, transform);
        return convert(raw, out T result) ? result : default;
    }
}

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

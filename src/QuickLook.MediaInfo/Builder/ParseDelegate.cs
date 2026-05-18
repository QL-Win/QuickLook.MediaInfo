namespace QuickLook.MediaInfo.Builder;

internal delegate bool ParseDelegate<T>(string source, out T result);

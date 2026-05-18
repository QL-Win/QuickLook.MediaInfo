namespace QuickLook.MediaInfo.Core;

[Flags]
public enum InfoFileOptions
{
    FileOption_Nothing    = 0x00,
    FileOption_NoRecursive = 0x01,
    FileOption_CloseAll   = 0x02,
    FileOption_Max        = 0x04,
}

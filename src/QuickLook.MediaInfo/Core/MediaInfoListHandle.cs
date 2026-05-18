using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace QuickLook.MediaInfo.Core;

/// <summary>
/// Low-level P/Invoke wrapper around the native MediaInfoList library.
/// </summary>
[SuppressMessage("Interoperability", "SYSLIB1054:Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time")]
[SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression")]
public sealed class MediaInfoListHandle : IDisposable
{
    #region Native imports

    [DllImport(Libs.DllName)]
    private static extern nint MediaInfoList_New();

    [DllImport(Libs.DllName)]
    private static extern void MediaInfoList_Delete(nint handle);

    [DllImport(Libs.DllName, CharSet = CharSet.Unicode)]
    private static extern nint MediaInfoList_Open(nint handle, string fileName, nint options);

    [DllImport(Libs.DllName)]
    private static extern nint MediaInfoListA_Open(nint handle, nint fileName, nint options);

    [DllImport(Libs.DllName)]
    private static extern void MediaInfoList_Close(nint handle, nint filePos);

    [DllImport(Libs.DllName)]
    private static extern nint MediaInfoList_Inform(nint handle, nint filePos, nint reserved);

    [DllImport(Libs.DllName)]
    private static extern nint MediaInfoListA_Inform(nint handle, nint filePos, nint reserved);

    [DllImport(Libs.DllName, CharSet = CharSet.Unicode)]
    private static extern nint MediaInfoList_Get(nint handle, nint filePos, nint streamKind, nint streamNumber, string parameter, nint kindOfInfo, nint kindOfSearch);

    [DllImport(Libs.DllName)]
    private static extern nint MediaInfoListA_Get(nint handle, nint filePos, nint streamKind, nint streamNumber, nint parameter, nint kindOfInfo, nint kindOfSearch);

    [DllImport(Libs.DllName)]
    private static extern nint MediaInfoList_GetI(nint handle, nint filePos, nint streamKind, nint streamNumber, nint parameter, nint kindOfInfo);

    [DllImport(Libs.DllName)]
    private static extern nint MediaInfoListA_GetI(nint handle, nint filePos, nint streamKind, nint streamNumber, nint parameter, nint kindOfInfo);

    [DllImport(Libs.DllName, CharSet = CharSet.Unicode)]
    private static extern nint MediaInfoList_Option(nint handle, string option, string value);

    [DllImport(Libs.DllName)]
    private static extern nint MediaInfoListA_Option(nint handle, nint option, nint value);

    [DllImport(Libs.DllName)]
    private static extern nint MediaInfoList_State_Get(nint handle);

    [DllImport(Libs.DllName)]
    private static extern nint MediaInfoList_Count_Get(nint handle, nint filePos, nint streamKind, nint streamNumber);

    #endregion Native imports

    private bool _disposed;
    private readonly bool _useAnsi;

    private nint Handle { get; }

    public MediaInfoListHandle()
    {
        try { Handle = MediaInfoList_New(); }
        catch { Handle = IntPtr.Zero; }

#if NETFRAMEWORK
        _useAnsi = false;
#elif NET8_0_OR_GREATER
        _useAnsi = !OperatingSystem.IsWindows();
#else
        _useAnsi = !RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
#endif
    }

    ~MediaInfoListHandle() => Dispose(false);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    [SuppressMessage("Style", "IDE0060:Remove unused parameter")]
    private void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (Handle != IntPtr.Zero)
        {
            MediaInfoList_Close(Handle, -1);
            MediaInfoList_Delete(Handle);
        }
        _disposed = true;
    }

    public int Open(string fileName, InfoFileOptions options = InfoFileOptions.FileOption_Nothing)
    {
        if (Handle == IntPtr.Zero) return 0;
        if (_useAnsi)
        {
            nint ptr = Marshal.StringToHGlobalAnsi(fileName);
            try { return (int)MediaInfoListA_Open(Handle, ptr, (nint)options); }
            finally { Marshal.FreeHGlobal(ptr); }
        }
        return (int)MediaInfoList_Open(Handle, fileName, (nint)options);
    }

    public void Close(int filePos = -1)
    {
        if (Handle == IntPtr.Zero) return;
        MediaInfoList_Close(Handle, filePos);
    }

    public string Inform(int filePos = 0)
    {
        if (Handle == IntPtr.Zero) return string.Empty;
        return _useAnsi
            ? Marshal.PtrToStringAnsi(MediaInfoListA_Inform(Handle, filePos, 0)) ?? string.Empty
            : Marshal.PtrToStringUni(MediaInfoList_Inform(Handle, filePos, 0)) ?? string.Empty;
    }

    public string Get(int filePos, StreamKind streamKind, int streamNumber, string parameter,
        InfoKind kindOfInfo = InfoKind.Text, InfoKind kindOfSearch = InfoKind.Name)
    {
        if (Handle == IntPtr.Zero) return string.Empty;
        if (_useAnsi)
        {
            nint paramPtr = Marshal.StringToHGlobalAnsi(parameter);
            try
            {
                return Marshal.PtrToStringAnsi(
                    MediaInfoListA_Get(Handle, filePos, (nint)streamKind, streamNumber, paramPtr,
                        (nint)kindOfInfo, (nint)kindOfSearch)) ?? string.Empty;
            }
            finally { Marshal.FreeHGlobal(paramPtr); }
        }
        return Marshal.PtrToStringUni(
            MediaInfoList_Get(Handle, filePos, (nint)streamKind, streamNumber, parameter,
                (nint)kindOfInfo, (nint)kindOfSearch)) ?? string.Empty;
    }

    public string Get(int filePos, StreamKind streamKind, int streamNumber, int parameter,
        InfoKind kindOfInfo = InfoKind.Text)
    {
        if (Handle == IntPtr.Zero) return string.Empty;
        return _useAnsi
            ? Marshal.PtrToStringAnsi(MediaInfoListA_GetI(Handle, filePos, (nint)streamKind, streamNumber, parameter, (nint)kindOfInfo)) ?? string.Empty
            : Marshal.PtrToStringUni(MediaInfoList_GetI(Handle, filePos, (nint)streamKind, streamNumber, parameter, (nint)kindOfInfo)) ?? string.Empty;
    }

    public string Option(string option, string value = "")
    {
        if (Handle == IntPtr.Zero) return string.Empty;
        if (_useAnsi)
        {
            nint optPtr = Marshal.StringToHGlobalAnsi(option);
            nint valPtr = Marshal.StringToHGlobalAnsi(value);
            try
            {
                return Marshal.PtrToStringAnsi(MediaInfoListA_Option(Handle, optPtr, valPtr)) ?? string.Empty;
            }
            finally
            {
                Marshal.FreeHGlobal(optPtr);
                Marshal.FreeHGlobal(valPtr);
            }
        }
        return Marshal.PtrToStringUni(MediaInfoList_Option(Handle, option, value)) ?? string.Empty;
    }

    public int StateGet()
    {
        if (Handle == IntPtr.Zero) return 0;
        return (int)MediaInfoList_State_Get(Handle);
    }

    public int CountGet(int filePos, StreamKind streamKind, int streamNumber = -1)
    {
        if (Handle == IntPtr.Zero) return 0;
        return (int)MediaInfoList_Count_Get(Handle, filePos, (nint)streamKind, streamNumber);
    }
}

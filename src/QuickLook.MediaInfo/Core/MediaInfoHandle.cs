using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace QuickLook.MediaInfo.Core;

/// <summary>
/// Low-level P/Invoke wrapper around the native MediaInfo library.
/// All public methods return empty strings when the library fails to load.
/// </summary>
[SuppressMessage("Interoperability", "SYSLIB1054:Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time")]
[SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression")]
public sealed class MediaInfoHandle : IDisposable
{
    #region Native imports

    [DllImport(Libs.DllName, CharSet = CharSet.Unicode)]
    private static extern nint MediaInfo_New();

    [DllImport(Libs.DllName)]
    private static extern void MediaInfo_Delete(nint handle);

    [DllImport(Libs.DllName, CharSet = CharSet.Unicode)]
    private static extern nint MediaInfo_Open(nint handle, string fileName);

    [DllImport(Libs.DllName)]
    private static extern nint MediaInfoA_Open(nint handle, nint fileName);

    [DllImport(Libs.DllName)]
    private static extern nint MediaInfo_Open_Buffer_Init(nint handle, long fileSize, long fileOffset);

    [DllImport(Libs.DllName)]
    private static extern nint MediaInfo_Open_Buffer_Continue(nint handle, nint buffer, nint bufferSize);

    [DllImport(Libs.DllName)]
    private static extern long MediaInfo_Open_Buffer_Continue_GoTo_Get(nint handle);

    [DllImport(Libs.DllName)]
    private static extern nint MediaInfo_Open_Buffer_Finalize(nint handle);

    [DllImport(Libs.DllName)]
    private static extern void MediaInfo_Close(nint handle);

    [DllImport(Libs.DllName)]
    private static extern nint MediaInfo_Inform(nint handle, nint reserved);

    [DllImport(Libs.DllName)]
    private static extern nint MediaInfoA_Inform(nint handle, nint reserved);

    [DllImport(Libs.DllName, CharSet = CharSet.Unicode)]
    private static extern nint MediaInfo_Get(nint handle, nint streamKind, nint streamNumber, string parameter, nint kindOfInfo, nint kindOfSearch);

    [DllImport(Libs.DllName)]
    private static extern nint MediaInfoA_Get(nint handle, nint streamKind, nint streamNumber, nint parameter, nint kindOfInfo, nint kindOfSearch);

    [DllImport(Libs.DllName)]
    private static extern nint MediaInfo_GetI(nint handle, nint streamKind, nint streamNumber, nint parameter, nint kindOfInfo);

    [DllImport(Libs.DllName)]
    private static extern nint MediaInfoA_GetI(nint handle, nint streamKind, nint streamNumber, nint parameter, nint kindOfInfo);

    [DllImport(Libs.DllName, CharSet = CharSet.Unicode)]
    private static extern nint MediaInfo_Option(nint handle, string option, string value);

    [DllImport(Libs.DllName)]
    private static extern nint MediaInfoA_Option(nint handle, nint option, nint value);

    [DllImport(Libs.DllName)]
    private static extern nint MediaInfo_State_Get(nint handle);

    [DllImport(Libs.DllName)]
    private static extern nint MediaInfo_Count_Get(nint handle, nint streamKind, nint streamNumber);

    #endregion Native imports

    private bool _disposed;
    private readonly bool _useAnsi;

    /// <summary>Gets the native handle. <see cref="IntPtr.Zero"/> when the library failed to load.</summary>
    public nint Handle { get; }

    public MediaInfoHandle()
    {
        try
        {
            Handle = MediaInfo_New();
        }
        catch
        {
            Handle = IntPtr.Zero;
        }

#if NETFRAMEWORK
        _useAnsi = !true;
#elif NET8_0_OR_GREATER
        _useAnsi = !OperatingSystem.IsWindows();
#else
        _useAnsi = !RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
#endif
    }

    ~MediaInfoHandle() => Dispose(false);

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
            MediaInfo_Close(Handle);
            MediaInfo_Delete(Handle);
        }
        _disposed = true;
    }

    // -- Open / Close ----------------------------------------------------------

    public nint Open(string fileName)
    {
        if (Handle == IntPtr.Zero) return IntPtr.Zero;
        if (_useAnsi)
        {
            nint ptr = Marshal.StringToHGlobalAnsi(fileName);
            try { return MediaInfoA_Open(Handle, ptr); }
            finally { Marshal.FreeHGlobal(ptr); }
        }
        return MediaInfo_Open(Handle, fileName);
    }

    public nint OpenBufferInit(long fileSize, long fileOffset)
    {
        if (Handle == IntPtr.Zero) return IntPtr.Zero;
        return MediaInfo_Open_Buffer_Init(Handle, fileSize, fileOffset);
    }

    public nint OpenBufferContinue(nint buffer, nint bufferSize)
    {
        if (Handle == IntPtr.Zero) return IntPtr.Zero;
        return MediaInfo_Open_Buffer_Continue(Handle, buffer, bufferSize);
    }

    public unsafe nint OpenBufferContinue(byte* buffer, int bufferSize)
    {
        if (Handle == IntPtr.Zero) return IntPtr.Zero;
        return MediaInfo_Open_Buffer_Continue(Handle, (nint)buffer, bufferSize);
    }

    public long OpenBufferContinueGoToGet()
    {
        if (Handle == IntPtr.Zero) return -1;
        return MediaInfo_Open_Buffer_Continue_GoTo_Get(Handle);
    }

    public nint OpenBufferFinalize()
    {
        if (Handle == IntPtr.Zero) return IntPtr.Zero;
        return MediaInfo_Open_Buffer_Finalize(Handle);
    }

    public void Close()
    {
        if (Handle == IntPtr.Zero) return;
        MediaInfo_Close(Handle);
    }

    // -- Inform ----------------------------------------------------------------

    public string Inform()
    {
        if (Handle == IntPtr.Zero) return "Unable to load MediaInfo library";
        return _useAnsi
            ? Marshal.PtrToStringAnsi(MediaInfoA_Inform(Handle, 0)) ?? string.Empty
            : Marshal.PtrToStringUni(MediaInfo_Inform(Handle, 0)) ?? string.Empty;
    }

    // -- Get -------------------------------------------------------------------

    public string Get(StreamKind streamKind, int streamNumber, string parameter,
        InfoKind kindOfInfo = InfoKind.Text, InfoKind kindOfSearch = InfoKind.Name)
    {
        if (Handle == IntPtr.Zero) return "Unable to load MediaInfo library";
        if (_useAnsi)
        {
            nint paramPtr = Marshal.StringToHGlobalAnsi(parameter);
            try
            {
                return Marshal.PtrToStringAnsi(
                    MediaInfoA_Get(Handle, (nint)streamKind, streamNumber, paramPtr,
                        (nint)kindOfInfo, (nint)kindOfSearch)) ?? string.Empty;
            }
            finally { Marshal.FreeHGlobal(paramPtr); }
        }
        return Marshal.PtrToStringUni(
            MediaInfo_Get(Handle, (nint)streamKind, streamNumber, parameter,
                (nint)kindOfInfo, (nint)kindOfSearch)) ?? string.Empty;
    }

    public string Get(StreamKind streamKind, int streamNumber, int parameter,
        InfoKind kindOfInfo = InfoKind.Text)
    {
        if (Handle == IntPtr.Zero) return "Unable to load MediaInfo library";
        return _useAnsi
            ? Marshal.PtrToStringAnsi(MediaInfoA_GetI(Handle, (nint)streamKind, streamNumber, parameter, (nint)kindOfInfo)) ?? string.Empty
            : Marshal.PtrToStringUni(MediaInfo_GetI(Handle, (nint)streamKind, streamNumber, parameter, (nint)kindOfInfo)) ?? string.Empty;
    }

    // -- Option ----------------------------------------------------------------

    public string Option(string option, string value = "")
    {
        if (Handle == IntPtr.Zero) return "Unable to load MediaInfo library";
        if (_useAnsi)
        {
            nint optPtr = Marshal.StringToHGlobalAnsi(option);
            nint valPtr = Marshal.StringToHGlobalAnsi(value);
            try
            {
                return Marshal.PtrToStringAnsi(MediaInfoA_Option(Handle, optPtr, valPtr)) ?? string.Empty;
            }
            finally
            {
                Marshal.FreeHGlobal(optPtr);
                Marshal.FreeHGlobal(valPtr);
            }
        }
        return Marshal.PtrToStringUni(MediaInfo_Option(Handle, option, value)) ?? string.Empty;
    }

    // -- State / Count ---------------------------------------------------------

    public int StateGet()
    {
        if (Handle == IntPtr.Zero) return 0;
        return (int)MediaInfo_State_Get(Handle);
    }

    public int CountGet(StreamKind streamKind, int streamNumber = -1)
    {
        if (Handle == IntPtr.Zero) return 0;
        return (int)MediaInfo_Count_Get(Handle, (nint)streamKind, streamNumber);
    }
}

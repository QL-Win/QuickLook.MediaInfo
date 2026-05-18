using System.Runtime.InteropServices;
using Win32Exception = System.ComponentModel.Win32Exception;

namespace QuickLook.MediaInfo.Core;

/// <summary>
/// Native library names and loader utilities for MediaInfo.
/// </summary>
public static class Libs
{
    public const string Windows = "MediaInfo.dll";
    public const string MacOS = "libmediainfo.dylib";
    public const string Linux = "libmediainfo.so.0";

    /// <summary>
    /// Sets the DLL search directory so that <c>MediaInfo.dll</c> is resolved from
    /// <paramref name="dllDirectory"/> at runtime (Windows only).
    /// </summary>
    /// <param name="dllDirectory">Directory containing MediaInfo.dll.</param>
    public static void LoadDll(string dllDirectory)
    {
#if NETFRAMEWORK
#elif NET8_0_OR_GREATER
        if (!OperatingSystem.IsWindows())
            return;
#else
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return;
#endif

        if (!SetDllDirectory(dllDirectory))
            throw new Win32Exception(Marshal.GetLastWin32Error());
    }

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetDllDirectory(string lpPathName);
}

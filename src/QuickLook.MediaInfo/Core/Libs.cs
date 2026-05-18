using System.IO;
using System.Runtime.InteropServices;

#if NET5_0_OR_GREATER
using System.Reflection;
using System.Threading;
#endif

using Win32Exception = System.ComponentModel.Win32Exception;

namespace QuickLook.MediaInfo.Core;

/// <summary>
/// Native library names and loader utilities for MediaInfo.
/// </summary>
public static class Libs
{
    /// <summary>Logical name used in all <see cref="DllImportAttribute"/> declarations.</summary>
    internal const string DllName = "MediaInfo";

    public const string Windows = "MediaInfo.dll";
    public const string MacOS = "libmediainfo.dylib";
    public const string Linux = "libmediainfo.so.0";

    /// <summary>
    /// Configures the native library search path so that <c>MediaInfo</c> is resolved
    /// from <paramref name="dllDirectory"/> at runtime on all platforms.
    /// On Windows (all frameworks): configures the DLL search directory.
    /// On Linux/macOS (.NET 5+): installs a <see cref="NativeLibrary"/> resolver.
    /// On netstandard builds on non-Windows: the library must be in the OS search path.
    /// </summary>
    /// <param name="dllDirectory">Directory containing the platform-appropriate MediaInfo native library.</param>
    public static void LoadDll(string dllDirectory)
    {
#if NETFRAMEWORK
        if (!SetDllDirectory(dllDirectory))
            throw new Win32Exception(Marshal.GetLastWin32Error());
#elif NET5_0_OR_GREATER || NETCOREAPP3_0_OR_GREATER
        NativeLibrary.SetDllImportResolver(
            typeof(Libs).Assembly,
            (name, _, _) =>
            {
                if (name != DllName) return IntPtr.Zero;
                return NativeLibrary.Load(GetLibraryPath(dllDirectory));
            });
#else
        // netstandard2.0 / netstandard2.1: use SetDllDirectory on Windows;
        // on other platforms the library must be in the OS search path.
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) &&
            !SetDllDirectoryW(dllDirectory))
            throw new Win32Exception(Marshal.GetLastWin32Error());
#endif
    }

    internal static string GetLibraryPath(string directory)
    {
#if NETFRAMEWORK
        return Path.Combine(directory, Windows);
#else
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return Path.Combine(directory, Windows);
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            return Path.Combine(directory, MacOS);
        return Path.Combine(directory, Linux);
#endif
    }

#if NETFRAMEWORK

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetDllDirectory(string lpPathName);

#else
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode,
        EntryPoint = "SetDllDirectoryW")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetDllDirectoryW(string lpPathName);
#endif
}

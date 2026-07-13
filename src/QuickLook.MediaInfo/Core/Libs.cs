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
    /// When <paramref name="dllDirectory"/> is <c>null</c>, attempts to auto-detect
    /// the library from well-known locations (see <see cref="ResolveDefaultDllDirectory"/>).
    /// </summary>
    /// <param name="dllDirectory">
    /// Directory containing the platform-appropriate MediaInfo native library,
    /// or <c>null</c> to search default paths.
    /// </param>
    public static void LoadDll(string? dllDirectory = null)
    {
        if (dllDirectory == null)
        {
            dllDirectory = ResolveDefaultDllDirectory();
            if (dllDirectory == null)
                return;
        }

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

    /// <summary>
    /// Attempts to resolve the native library directory from well-known locations.
    /// <list type="bullet">
    ///   <item><c>{AppContext.BaseDirectory}/runtimes/{rid}/native/</c></item>
    ///   <item><c>{assemblyDir}/runtimes/{rid}/native/</c></item>
    ///   <item><c>{NuGet package root}/runtimes/{rid}/native/</c> (when assembly is under <c>lib/{tfm}/</c>)</item>
    /// </list>
    /// </summary>
    internal static string? ResolveDefaultDllDirectory()
    {
        string rid = GetRuntimeIdentifier();
        string?[] bases = GetSearchBaseDirectories();

        foreach (var dir in bases)
        {
            if (dir == null) continue;
            string candidate = Path.Combine(dir, "runtimes", rid, "native");
            if (File.Exists(GetLibraryPath(candidate)))
                return candidate;
        }

        return null;
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

    internal static string GetRuntimeIdentifier()
    {
#if NETFRAMEWORK
        // .NET Framework is Windows-only; use IsWow64Process2 to detect native
        // machine architecture, which correctly identifies arm64 even when the
        // process runs in emulation (x64 or x86 on arm64).
        string arch = GetWindowsNativeArch();
        return $"win-{arch}";
#elif NET5_0_OR_GREATER
        return NormalizeRid(RuntimeInformation.RuntimeIdentifier);
#else
        // netstandard2.0+ / netcoreapp3.1 have ProcessArchitecture
        string os = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "win" :
                     RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "osx" : "linux";
        string arch = RuntimeInformation.ProcessArchitecture switch
        {
            Architecture.X86 => "x86",
            Architecture.X64 => "x64",
            Architecture.Arm64 => "arm64",
            Architecture.Arm => "arm",
            _ => "x64"
        };
        return $"{os}-{arch}";
#endif
    }

    private static string NormalizeRid(string rid)
    {
        if (rid.StartsWith("win") && rid.Length > 5 && char.IsDigit(rid[3]))
        {
            int dashIndex = rid.IndexOf('-', 3);
            if (dashIndex > 0)
                return "win" + rid.Substring(dashIndex);
        }

        if (!rid.StartsWith("linux") && !rid.StartsWith("osx") && !rid.StartsWith("win"))
        {
            int lastDash = rid.LastIndexOf('-');
            if (lastDash > 0)
            {
                string arch = rid.Substring(lastDash + 1);
                return $"linux-{arch}";
            }
        }

        return rid;
    }

    private static string?[] GetSearchBaseDirectories()
    {
        string appBase = AppContext.BaseDirectory;
        string? assemblyDir = Path.GetDirectoryName(typeof(Libs).Assembly.Location);

        string? packageRoot = null;
        if (assemblyDir != null)
        {
            var libDir = Directory.GetParent(assemblyDir);
            packageRoot = libDir?.Parent?.FullName;
        }

        if (packageRoot != null && packageRoot != appBase && packageRoot != assemblyDir)
            return new string?[] { appBase, assemblyDir, packageRoot };

        return new string?[] { appBase, assemblyDir };
    }

#if NETFRAMEWORK

    // Machine architecture constants used by IsWow64Process2
    private const ushort IMAGE_FILE_MACHINE_I386 = 0x014C;
    private const ushort IMAGE_FILE_MACHINE_AMD64 = 0x8664;
    private const ushort IMAGE_FILE_MACHINE_ARM64 = 0xAA64;

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool IsWow64Process2(
        IntPtr hProcess, out ushort pProcessMachine, out ushort pNativeMachine);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetDllDirectory(string lpPathName);

    private static string GetWindowsNativeArch()
    {
        var hProc = System.Diagnostics.Process.GetCurrentProcess().Handle;
        if (IsWow64Process2(hProc, out _, out ushort nativeMachine))
        {
            return nativeMachine switch
            {
                IMAGE_FILE_MACHINE_I386 => "x86",
                IMAGE_FILE_MACHINE_AMD64 => "x64",
                IMAGE_FILE_MACHINE_ARM64 => "arm64",
                _ => Environment.Is64BitProcess ? "x64" : "x86"
            };
        }
        // Fallback if API is unavailable (e.g. Windows < 10)
        return Environment.Is64BitProcess ? "x64" : "x86";
    }

#else
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode,
        EntryPoint = "SetDllDirectoryW")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetDllDirectoryW(string lpPathName);
#endif
}

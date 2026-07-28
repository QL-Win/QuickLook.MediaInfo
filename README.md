# QuickLook.MediaInfo

[![NuGet](https://img.shields.io/nuget/v/QuickLook.MediaInfo.svg)](https://nuget.org/packages/QuickLook.MediaInfo)
[![Platform](https://img.shields.io/badge/platform-Windows%20%7C%20macOS%20%7C%20Linux-blue)](https://github.com/QL-Win/QuickLook.MediaInfo)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

A .NET wrapper for [MediaInfoLib](https://mediaarea.net/en/MediaInfo) from [MediaArea.net](https://mediaarea.net), providing both low-level P/Invoke bindings and a high-level typed model for media metadata.

This project is originally derived from [lemutec/MediaInfoDLL](https://github.com/lemutec/MediaInfoDLL) (raw P/Invoke wrapper) and [yartat/MP-MediaInfo](https://github.com/yartat/MP-MediaInfo) (typed model), then significantly extended by the [QL-Win](https://github.com/QL-Win) team for use with [QuickLook](https://github.com/QL-Win/QuickLook).

## Features

- **Dual API**: Low-level `MediaInfoHandle`/`MediaInfoNative` for raw access, and high-level `MediaInfoWrapper` for structured metadata
- **Rich Typed Model**: Strongly-typed stream objects with enums for codecs, HDR formats, chroma subsampling, color spaces, stereo modes, and more
- **Cross-Platform**: Windows (x86, x64, ARM64) and macOS (x64) native library support
- **Stream-Based Parsing**: Parse media from `System.IO.Stream` (both seekable and non-seekable), not just file paths
- **Multi-Language**: 40 built-in languages via embedded CSV resources
- **Multiple Output Formats**: Text, Tree, XML, MAXML, JSON, HTML, CSV, and custom templates
- **Fluent API**: Extension methods for chained operations (`WithOpen`, `WithOption`)
- **Multi-File**: `MediaInfoListHandle` for batch operations
- **12 Target Frameworks**: .NET 4.6.2+, .NET Standard 2.0/2.1, .NET Core 3.1, .NET 5-10

## Installation

```shell
dotnet add package QuickLook.MediaInfo
```

### Native Runtime Packages

The managed library requires the native `MediaInfo` DLL. Include the platform-specific package:

| Package | Platform |
|---------|----------|
| `QuickLook.MediaInfo.Native.win-x64` | Windows x64 |
| `QuickLook.MediaInfo.Native.win-x86` | Windows x86 |
| `QuickLook.MediaInfo.Native.win-arm64` | Windows ARM64 |
| `QuickLook.MediaInfo.Native.osx-x64` | macOS x64 |

```shell
dotnet add package QuickLook.MediaInfo.Native.win-x64
```

## Quick Start

### High-Level API (Typed Model)

```csharp
using QuickLook.MediaInfo;

var media = new MediaInfoWrapper("video.mp4");

if (media.Success)
{
    Console.WriteLine($"Duration: {media.Duration} ms");
    Console.WriteLine($"Size: {media.Size} bytes");

    // Video
    if (media.HasVideo)
    {
        var video = media.BestVideoStream;
        Console.WriteLine($"Video: {video.Codec} {video.Width}x{video.Height} @ {video.FrameRate}fps");
        Console.WriteLine($"HDR: {video.Hdr}, Interlaced: {video.Interlaced}");
    }

    // Audio
    foreach (var audio in media.AudioStreams)
    {
        Console.WriteLine($"Audio: {audio.Codec} {audio.SamplingRate}Hz {audio.Channels}ch");
    }

    // Subtitles
    foreach (var sub in media.Subtitles)
    {
        Console.WriteLine($"Subtitle: {sub.Codec} ({sub.Language})");
    }

    // Chapters
    foreach (var ch in media.Chapters)
    {
        Console.WriteLine($"Chapter: {ch.Name} @ {ch.StartTime}");
    }
}
```

### Stream-Based Parsing

```csharp
using var fileStream = File.OpenRead("video.mkv");
var media = new MediaInfoWrapper(fileStream);

if (media.Success)
{
    Console.WriteLine($"Duration: {media.Duration} ms");
    Console.WriteLine($"Video streams: {media.VideoStreams.Count}");
    Console.WriteLine($"Audio streams: {media.AudioStreams.Count}");
}
```

### Output in Specific Format & Language

```csharp
string xml = MediaInfoWrapper.GetInform(
    "video.mp4",
    MediaInfoOptions.InformFormat.XML,
    MediaInfoLanguage.ChineseSimplified);

Console.WriteLine(xml);
```

### Multi-Language Output

Set the output language via the `Language` option. The library ships with 40 built-in languages.

```csharp
using QuickLook.MediaInfo.Core;

using var mi = new MediaInfoNative();
mi.Open("video.mp4");

// Set language to Chinese Simplified
mi.Option("Language", MediaInfoLanguage.ChineseSimplified.ToIso639());
Console.WriteLine(mi.Inform());

// Set language to Japanese
mi.Option("Language", MediaInfoLanguage.Japanese.ToIso639());
Console.WriteLine(mi.Inform());

// Convert language to ISO code string
string code = MediaInfoLanguage.French.ToCode();       // "fr"
string iso639 = MediaInfoLanguage.German.ToIso639();   // full CSV content
var culture = MediaInfoLanguage.Russian.ToCultureInfo(); // CultureInfo
```

Or with `MediaInfoWrapper.GetInform()`:

```csharp
string result = MediaInfoWrapper.GetInform(
    "video.mp4",
    MediaInfoOptions.InformFormat.Text,
    MediaInfoLanguage.ChineseSimplified);
```

### Low-Level API

```csharp
using QuickLook.MediaInfo.Core;

using var mi = new MediaInfoNative();
mi.Option("Cover_Data", "base64");
mi.Open("video.mp4");

string format = mi.Get(StreamKind.Video, 0, "Format");
int audioCount = mi.CountGet(StreamKind.Audio);
string inform = mi.Inform();

mi.Close();
```

### Low-Level Handle with Fluent API

```csharp
using var handle = new MediaInfoHandle();

string version = handle.WithOption("Info_Version").Inform();
int audioStreams = handle
    .WithOpen("video.mp4")
    .CountGet(StreamKind.Audio);
```

## API Overview

### Namespace: `QuickLook.MediaInfo`

| Type | Description |
|------|-------------|
| `MediaInfoWrapper` | High-level typed model (file path or stream input) |
| `MediaInfoNative` | Convenient low-level wrapper over `MediaInfoHandle` |

### Namespace: `QuickLook.MediaInfo.Core`

| Type | Description |
|------|-------------|
| `MediaInfoHandle` | Low-level P/Invoke wrapper for `MediaInfo_*` API |
| `MediaInfoListHandle` | Low-level P/Invoke wrapper for `MediaInfoList_*` API (multi-file) |
| `StreamKind` | Enum: General, Video, Audio, Text, Other, Image, Menu |
| `Status` | Flags enum: None, Accepted, Filled, Updated, Finalized |
| `InfoKind` | Enum: Name, Text, Measure, Options, NameText, MeasureText, Info, HowTo |
| `InfoOptions` | Enum: ShowInInform, Support, ShowInSupported, TypeOfValue |
| `InfoFileOptions` | Flags enum for file opening options |
| `MediaInfoLanguage` | Enum for 40 languages with ISO code mapping |
| `MediaInfoOptions` | Static class with ~50 MediaInfo option string constants (Complete, Internet, ParseSpeed, Language, Inform, etc.) |
| `MediaInfoOptions.InformFormat` | Enum: Text, Tree, XML, MAXML, JSON, HTML, CSV, Custom |
| `Libs` | Static native library loader with auto-RID detection |
| `LanguageExtension` | Extension methods for `MediaInfoLanguage` and `CultureInfo` |

### Namespace: `QuickLook.MediaInfo.Model`

| Type | Description |
|------|-------------|
| `MediaStream` | Abstract base class for all stream types |
| `LanguageMediaStream` | Abstract base with language/LCID properties |
| `VideoStream` | Typed video stream with codec, resolution, framerate, HDR, chroma, stereo mode, etc. |
| `AudioStream` | Typed audio stream with codec, sampling rate, channels, bitrate, bit depth, etc. |
| `SubtitleStream` | Typed subtitle stream with codec, language, duration |
| `ChapterStream` | Typed chapter with start/end time |
| `MenuStream` | Typed menu stream |
| `VideoCodec` | Enum with 46 codecs (Undefined..Hap) |
| `AudioCodec` | Enum with 88 codecs (Undefined..Mac6) |
| `SubtitleCodec` | Enum with 22 codecs |
| `AspectRatio` | Enum with 9 values (Opaque..CinemaScope) |
| `FrameRateMode`, `BitrateMode` | Enums for CFR/VFR, CBR/VBR/CQ |
| `ColorSpace`, `ChromaSubSampling`, `TransferCharacteristic` | Video color property enums |
| `StereoMode` | Enum with 19 stereo layout values |
| `Hdr` | Enum: None, DolbyVision, HDR10, HDR10Plus, SLHDR1, HLG, HdrVivid |

## Native Library Loading

The library auto-detects the platform runtime identifier (RID) and searches for the native `MediaInfo.dll` (or `libmediainfo.dylib`) in these locations:

1. `{AppBase}/runtimes/{rid}/native/`
2. `{AssemblyDir}/runtimes/{rid}/native/`
3. NuGet package cache paths

On .NET 5+, `NativeLibrary.SetDllImportResolver` is used for clean resolution. On .NET Framework, `SetDllDirectoryW` is used.

You can also specify a custom path:

```csharp
var mi = new MediaInfoNative(@"C:\path\to\native\dll");
// or
Libs.LoadDll(@"C:\path\to\native\dll");
```

## Build & Packaging

The repository produces two kinds of NuGet packages:

| Package | Description |
|---------|-------------|
| `QuickLook.MediaInfo` | The managed library (multi-target: 12 frameworks) |
| `QuickLook.MediaInfo.Native.win-x64` | Native DLL for Windows x64 |
| `QuickLook.MediaInfo.Native.win-x86` | Native DLL for Windows x86 |
| `QuickLook.MediaInfo.Native.win-arm64` | Native DLL for Windows ARM64 |
| `QuickLook.MediaInfo.Native.osx-x64` | Native library for macOS x64 |

Build scripts (PowerShell):

- `nuget_restore.ps1` — Restore all packages
- `nuget_pack.ps1` — Build managed library and pack native `.nuspec` files
- `nuget_push.ps1` — Push all packages to nuget.org
- `nuget_lib.ps1` — Download & extract MediaInfo native libraries from MediaArea archives

## Supported Frameworks

.NET 4.6.2, 4.7.2, 4.8, .NET Standard 2.0, .NET Standard 2.1, .NET Core 3.1, .NET 5, 6, 7, 8, 9, 10.

## References

https://github.com/MediaArea/MediaInfoLib/blob/master/Source/MediaInfo/MediaInfo_Config.h

https://github.com/MediaArea/MediaInfoLib/blob/master/Source/MediaInfo/File__Analyse_Automatic.h

https://github.com/MediaArea/MediaInfo/tree/master/Source/Resource/Plugin/Language

https://github.com/MediaArea/MediaInfo/blob/master/Source/Resource/Language.csv

## License

Copyright (c) 2025-2026 QL-Win Contributors. Licensed under the [MIT License](LICENSE).

This project uses [MediaInfoLib](https://github.com/MediaArea/MediaInfoLib) by MediaArea.net SARL (BSD-2-Clause).

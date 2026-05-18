using QuickLook.MediaInfo.Model;
using Xunit;

namespace QuickLook.MediaInfo.Tests;

/// <summary>
/// Tests for <see cref="MediaInfoWrapper"/> high-level properties using example.mp4.
/// MediaInfo.dll must be present in the output directory for these tests to pass.
/// </summary>
public class MediaInfoWrapperTests
{
    private static readonly string MediaFile = Path.Combine(AppContext.BaseDirectory, "example.mp4");

    // -------------------------------------------------------------------------
    // Constructor / basic parse
    // -------------------------------------------------------------------------

    [Fact]
    public void Constructor_ValidFile_SuccessTrue()
    {
        var wrapper = new MediaInfoWrapper(MediaFile);
        Assert.True(wrapper.Success);
    }

    [Fact]
    public void Constructor_NonExistentFile_SuccessFalse()
    {
        var wrapper = new MediaInfoWrapper("nonexistent_file_that_does_not_exist.mp4");
        Assert.False(wrapper.Success);
    }

    [Fact]
    public void Constructor_EmptyString_SuccessFalse()
    {
        var wrapper = new MediaInfoWrapper(string.Empty);
        Assert.False(wrapper.Success);
    }

    [Fact]
    public void Constructor_ValidFile_VersionNotEmpty()
    {
        var wrapper = new MediaInfoWrapper(MediaFile);
        Assert.False(string.IsNullOrWhiteSpace(wrapper.Version));
    }

    // -------------------------------------------------------------------------
    // Container-level properties
    // -------------------------------------------------------------------------

    [Fact]
    public void Duration_GreaterThanZero()
    {
        var wrapper = new MediaInfoWrapper(MediaFile);
        Assert.True(wrapper.Duration > 0);
    }

    [Fact]
    public void Size_GreaterThanZero()
    {
        var wrapper = new MediaInfoWrapper(MediaFile);
        Assert.True(wrapper.Size > 0);
    }

    // -------------------------------------------------------------------------
    // Video stream presence and properties
    // -------------------------------------------------------------------------

    [Fact]
    public void HasVideo_IsTrue()
    {
        var wrapper = new MediaInfoWrapper(MediaFile);
        Assert.True(wrapper.HasVideo);
    }

    [Fact]
    public void VideoStreams_NotEmpty()
    {
        var wrapper = new MediaInfoWrapper(MediaFile);
        Assert.NotEmpty(wrapper.VideoStreams);
    }

    [Fact]
    public void BestVideoStream_NotNull()
    {
        var wrapper = new MediaInfoWrapper(MediaFile);
        Assert.NotNull(wrapper.BestVideoStream);
    }

    [Fact]
    public void Width_GreaterThanZero()
    {
        var wrapper = new MediaInfoWrapper(MediaFile);
        Assert.True(wrapper.Width > 0);
    }

    [Fact]
    public void Height_GreaterThanZero()
    {
        var wrapper = new MediaInfoWrapper(MediaFile);
        Assert.True(wrapper.Height > 0);
    }

    [Fact]
    public void Framerate_GreaterThanZero()
    {
        var wrapper = new MediaInfoWrapper(MediaFile);
        Assert.True(wrapper.Framerate > 0);
    }

    [Fact]
    public void VideoCodec_NotUndefined()
    {
        var wrapper = new MediaInfoWrapper(MediaFile);
        Assert.NotEqual(VideoCodec.Undefined, wrapper.VideoCodec);
    }

    [Fact]
    public void VideoResolution_NotUnknown()
    {
        var wrapper = new MediaInfoWrapper(MediaFile);
        Assert.NotEqual("Unknown", wrapper.VideoResolution);
    }

    [Fact]
    public void IsInterlaced_HasValue()
    {
        // Just verifies the property is accessible without throwing.
        var wrapper = new MediaInfoWrapper(MediaFile);
        _ = wrapper.IsInterlaced;
        _ = wrapper.ScanType;
    }

    [Fact]
    public void AspectRatio_HasValue()
    {
        var wrapper = new MediaInfoWrapper(MediaFile);
        _ = wrapper.AspectRatio; // must not throw
    }

    // -------------------------------------------------------------------------
    // Audio stream presence and properties
    // -------------------------------------------------------------------------

    [Fact]
    public void AudioStreams_NotEmpty()
    {
        var wrapper = new MediaInfoWrapper(MediaFile);
        Assert.NotEmpty(wrapper.AudioStreams);
    }

    [Fact]
    public void BestAudioStream_NotNull()
    {
        var wrapper = new MediaInfoWrapper(MediaFile);
        Assert.NotNull(wrapper.BestAudioStream);
    }

    [Fact]
    public void AudioCodec_NotUndefined()
    {
        var wrapper = new MediaInfoWrapper(MediaFile);
        Assert.NotEqual(AudioCodec.Undefined, wrapper.AudioCodec);
    }

    [Fact]
    public void AudioRate_GreaterThanZero()
    {
        var wrapper = new MediaInfoWrapper(MediaFile);
        Assert.True(wrapper.AudioRate > 0);
    }

    [Fact]
    public void AudioChannels_GreaterThanZero()
    {
        var wrapper = new MediaInfoWrapper(MediaFile);
        Assert.True(wrapper.AudioChannels > 0);
    }

    [Fact]
    public void AudioChannelsFriendly_NotEmpty()
    {
        var wrapper = new MediaInfoWrapper(MediaFile);
        Assert.NotEmpty(wrapper.AudioChannelsFriendly);
    }

    // -------------------------------------------------------------------------
    // Stream parsing via Stream overload
    // -------------------------------------------------------------------------

    [Fact]
    public void Constructor_SeekableStream_SuccessTrue()
    {
        using var stream = File.OpenRead(MediaFile);
        var wrapper = new MediaInfoWrapper(stream);
        Assert.True(wrapper.Success);
    }

    [Fact]
    public void Constructor_SeekableStream_VideoDetected()
    {
        using var stream = File.OpenRead(MediaFile);
        var wrapper = new MediaInfoWrapper(stream);
        Assert.True(wrapper.HasVideo);
    }

    [Fact]
    public void Constructor_SeekableStream_AudioDetected()
    {
        using var stream = File.OpenRead(MediaFile);
        var wrapper = new MediaInfoWrapper(stream);
        Assert.NotEmpty(wrapper.AudioStreams);
    }

    // -------------------------------------------------------------------------
    // Video stream detailed properties
    // -------------------------------------------------------------------------

    [Fact]
    public void BestVideoStream_HasPositiveStreamPosition()
    {
        var wrapper = new MediaInfoWrapper(MediaFile);
        Assert.True(wrapper.BestVideoStream!.StreamPosition >= 0);
    }

    [Fact]
    public void BestVideoStream_KindIsVideo()
    {
        var wrapper = new MediaInfoWrapper(MediaFile);
        Assert.Equal(MediaStreamKind.Video, wrapper.BestVideoStream!.Kind);
    }

    // -------------------------------------------------------------------------
    // Audio stream detailed properties
    // -------------------------------------------------------------------------

    [Fact]
    public void BestAudioStream_KindIsAudio()
    {
        var wrapper = new MediaInfoWrapper(MediaFile);
        Assert.Equal(MediaStreamKind.Audio, wrapper.BestAudioStream!.Kind);
    }
}

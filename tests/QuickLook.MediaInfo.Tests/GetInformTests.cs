using QuickLook.MediaInfo.Core;
using Xunit;

namespace QuickLook.MediaInfo.Tests;

/// <summary>
/// Tests for <see cref="MediaInfoWrapper.GetInform"/> and <see cref="MediaInfoWrapper.GetLibraryVersion"/>.
/// Covers all six built-in Inform formats, multi-language output, custom templates, and version detection.
/// MediaInfo.dll must be present in the output directory for these tests to pass.
/// </summary>
public class GetInformTests
{
    private static readonly string MediaFile = Path.Combine(AppContext.BaseDirectory, "example.mp4");

    // -------------------------------------------------------------------------
    // Library version
    // -------------------------------------------------------------------------

    [Fact]
    public void GetLibraryVersion_NotNullOrEmpty()
    {
        string? version = MediaInfoWrapper.GetLibraryVersion();
        Assert.False(string.IsNullOrEmpty(version));
    }

    [Fact]
    public void GetLibraryVersion_ContainsVersionNumber()
    {
        string? version = MediaInfoWrapper.GetLibraryVersion();
        // e.g. "MediaInfoLib - v24.01" or "v24.01"
        Assert.Matches(@"\d+\.\d+", version ?? string.Empty);
    }

    // -------------------------------------------------------------------------
    // Text format (default)
    // -------------------------------------------------------------------------

    [Fact]
    public void GetInform_DefaultTextFormat_NotEmpty()
    {
        string result = MediaInfoWrapper.GetInform(MediaFile);
        Assert.False(string.IsNullOrWhiteSpace(result));
    }

    [Fact]
    public void GetInform_TextFormat_ContainsGeneralSection()
    {
        string result = MediaInfoWrapper.GetInform(MediaFile, MediaInfoOptions.InformFormat.Text);
        Assert.Contains("General", result, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void GetInform_TextFormat_ContainsVideoSection()
    {
        string result = MediaInfoWrapper.GetInform(MediaFile, MediaInfoOptions.InformFormat.Text);
        Assert.Contains("Video", result, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void GetInform_TextFormat_ContainsAudioSection()
    {
        string result = MediaInfoWrapper.GetInform(MediaFile, MediaInfoOptions.InformFormat.Text);
        Assert.Contains("Audio", result, StringComparison.OrdinalIgnoreCase);
    }

    // -------------------------------------------------------------------------
    // XML format
    // -------------------------------------------------------------------------

    [Fact]
    public void GetInform_XmlFormat_NotEmpty()
    {
        string result = MediaInfoWrapper.GetInform(MediaFile, MediaInfoOptions.InformFormat.XML);
        Assert.False(string.IsNullOrWhiteSpace(result));
    }

    [Fact]
    public void GetInform_XmlFormat_IsValidXml()
    {
        string result = MediaInfoWrapper.GetInform(MediaFile, MediaInfoOptions.InformFormat.XML);
        var doc = new System.Xml.XmlDocument();
        // Must not throw
        doc.LoadXml(result);
        Assert.NotNull(doc.DocumentElement);
    }

    [Fact]
    public void GetInform_XmlFormat_RootElementIsMediaInfo()
    {
        string result = MediaInfoWrapper.GetInform(MediaFile, MediaInfoOptions.InformFormat.XML);
        var doc = new System.Xml.XmlDocument();
        doc.LoadXml(result);
        Assert.Equal("Mediainfo", doc.DocumentElement!.LocalName, ignoreCase: true);
    }

    // -------------------------------------------------------------------------
    // JSON format
    // -------------------------------------------------------------------------

    [Fact]
    public void GetInform_JsonFormat_NotEmpty()
    {
        string result = MediaInfoWrapper.GetInform(MediaFile, MediaInfoOptions.InformFormat.JSON);
        Assert.False(string.IsNullOrWhiteSpace(result));
    }

    [Fact]
    public void GetInform_JsonFormat_StartsWithBrace()
    {
        string result = MediaInfoWrapper.GetInform(MediaFile, MediaInfoOptions.InformFormat.JSON).TrimStart();
        Assert.True(result.StartsWith('{'), $"Expected JSON object, got: {result[..Math.Min(40, result.Length)]}");
    }

    [Fact]
    public void GetInform_JsonFormat_ContainsMediaKey()
    {
        string result = MediaInfoWrapper.GetInform(MediaFile, MediaInfoOptions.InformFormat.JSON);
        Assert.Contains("\"media\"", result, StringComparison.OrdinalIgnoreCase);
    }

    // -------------------------------------------------------------------------
    // CSV format
    // -------------------------------------------------------------------------

    [Fact]
    public void GetInform_CsvFormat_NotEmpty()
    {
        string result = MediaInfoWrapper.GetInform(MediaFile, MediaInfoOptions.InformFormat.CSV);
        Assert.False(string.IsNullOrWhiteSpace(result));
    }

    [Fact]
    public void GetInform_CsvFormat_ContainsSemicolon()
    {
        // MediaInfo CSV uses semicolons as separators
        string result = MediaInfoWrapper.GetInform(MediaFile, MediaInfoOptions.InformFormat.CSV);
        Assert.Contains(';', result);
    }

    // -------------------------------------------------------------------------
    // HTML format
    // -------------------------------------------------------------------------

    [Fact]
    public void GetInform_HtmlFormat_NotEmpty()
    {
        string result = MediaInfoWrapper.GetInform(MediaFile, MediaInfoOptions.InformFormat.HTML);
        Assert.False(string.IsNullOrWhiteSpace(result));
    }

    [Fact]
    public void GetInform_HtmlFormat_ContainsHtmlTag()
    {
        string result = MediaInfoWrapper.GetInform(MediaFile, MediaInfoOptions.InformFormat.HTML);
        Assert.Contains("<html", result, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void GetInform_HtmlFormat_ContainsTableTag()
    {
        string result = MediaInfoWrapper.GetInform(MediaFile, MediaInfoOptions.InformFormat.HTML);
        Assert.Contains("<table", result, StringComparison.OrdinalIgnoreCase);
    }

    // -------------------------------------------------------------------------
    // MAXML format
    // -------------------------------------------------------------------------

    [Fact]
    public void GetInform_MaxmlFormat_NotEmpty()
    {
        string result = MediaInfoWrapper.GetInform(MediaFile, MediaInfoOptions.InformFormat.MAXML);
        Assert.False(string.IsNullOrWhiteSpace(result));
    }

    [Fact]
    public void GetInform_MaxmlFormat_IsValidXml()
    {
        string result = MediaInfoWrapper.GetInform(MediaFile, MediaInfoOptions.InformFormat.MAXML);
        var doc = new System.Xml.XmlDocument();
        doc.LoadXml(result);
        Assert.NotNull(doc.DocumentElement);
    }

    // -------------------------------------------------------------------------
    // Multi-language support
    // -------------------------------------------------------------------------

    [Theory]
    [InlineData(MediaInfoLanguage.English)]
    [InlineData(MediaInfoLanguage.French)]
    [InlineData(MediaInfoLanguage.German)]
    [InlineData(MediaInfoLanguage.Japanese)]
    [InlineData(MediaInfoLanguage.ChineseSimplified)]
    [InlineData(MediaInfoLanguage.ChineseTraditional)]
    [InlineData(MediaInfoLanguage.Korean)]
    [InlineData(MediaInfoLanguage.Russian)]
    [InlineData(MediaInfoLanguage.Spanish)]
    [InlineData(MediaInfoLanguage.Portuguese)]
    public void GetInform_WithLanguage_NotEmpty(MediaInfoLanguage lang)
    {
        string result = MediaInfoWrapper.GetInform(MediaFile, MediaInfoOptions.InformFormat.Text, lang);
        Assert.False(string.IsNullOrWhiteSpace(result));
    }

    [Fact]
    public void GetInform_NullLanguage_SameAsDefault()
    {
        string withNull = MediaInfoWrapper.GetInform(MediaFile, MediaInfoOptions.InformFormat.Text, null);
        string withDefault = MediaInfoWrapper.GetInform(MediaFile, MediaInfoOptions.InformFormat.Text);
        Assert.Equal(withDefault, withNull);
    }

    // -------------------------------------------------------------------------
    // Custom template (Inform template)
    // -------------------------------------------------------------------------

    [Fact]
    public void GetInform_CustomTemplate_FormatField_ReturnsFormat()
    {
        // "General;%Format%" returns the container format, e.g. "MPEG-4"
        string result = MediaInfoWrapper.GetInform(
            MediaFile,
            MediaInfoOptions.InformFormat.Custom,
            null,
            "General;%Format%");
        Assert.False(string.IsNullOrWhiteSpace(result));
    }

    [Fact]
    public void GetInform_CustomTemplate_MultipleFields_ReturnsBothFields()
    {
        string result = MediaInfoWrapper.GetInform(
            MediaFile,
            MediaInfoOptions.InformFormat.Custom,
            null,
            "General;%Format%|%Duration%");
        Assert.Contains('|', result);
    }

    [Fact]
    public void GetInform_CustomTemplate_VideoCodecField_NotEmpty()
    {
        string result = MediaInfoWrapper.GetInform(
            MediaFile,
            MediaInfoOptions.InformFormat.Custom,
            null,
            "Video;%Format%");
        Assert.False(string.IsNullOrWhiteSpace(result));
    }
}

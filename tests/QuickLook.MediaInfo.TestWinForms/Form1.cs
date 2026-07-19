using QuickLook.MediaInfo;
using QuickLook.MediaInfo.Core;

namespace QuickLook.MediaInfo.TestWinForms;

public class Form1 : Form
{
    private RichTextBox richTextBox1 = null!;

    public Form1()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        richTextBox1 = new RichTextBox();
        SuspendLayout();
        richTextBox1.Location = new Point(0, 0);
        richTextBox1.Name = "richTextBox1";
        richTextBox1.Size = new Size(768, 512);
        richTextBox1.TabIndex = 0;
        richTextBox1.Text = "";
        ClientSize = new Size(770, 514);
        Controls.Add(richTextBox1);
        FormBorderStyle = FormBorderStyle.Fixed3D;
        Name = "Form1";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "How to use QuickLook.MediaInfo";
        Load += Form1_Load;
        ResumeLayout(false);
    }

    private void Form1_Load(object? sender, EventArgs e)
    {
        string ToDisplay;
        using var MI = new MediaInfoNative();

        ToDisplay = MI.Option("Info_Version", "0.7.0.0;MediaInfoDLL_Example_CS;0.7.0.0");
        if (ToDisplay.Length == 0)
        {
            richTextBox1.Text = "MediaInfo.Dll: this version of the DLL is not compatible";
            return;
        }

        ToDisplay += "\r\n\r\nInfo_Parameters\r\n";
        ToDisplay += MI.Option("Info_Parameters");

        ToDisplay += "\r\n\r\nInfo_Capacities\r\n";
        ToDisplay += MI.Option("Info_Capacities");

        ToDisplay += "\r\n\r\nInfo_Codecs\r\n";
        ToDisplay += MI.Option("Info_Codecs");

        ToDisplay += "\r\n\r\nOpen\r\n";
        MI.Open("Example.ogg");

        ToDisplay += "\r\n\r\nInform with Complete=false\r\n";
        MI.Option("Complete");
        ToDisplay += MI.Inform();

        ToDisplay += "\r\n\r\nInform with Complete=true\r\n";
        MI.Option("Complete", "1");
        ToDisplay += MI.Inform();

        ToDisplay += "\r\n\r\nCustom Inform\r\n";
        MI.Option("Inform", "General;File size is %FileSize% bytes");
        ToDisplay += MI.Inform();

        ToDisplay += "\r\n\r\nGet with Stream=General and Parameter='FileSize'\r\n";
        ToDisplay += MI.Get(StreamKind.General, 0, "FileSize");

        ToDisplay += "\r\n\r\nGet with Stream=General and Parameter=46\r\n";
        ToDisplay += MI.Get(StreamKind.General, 0, 46);

        ToDisplay += "\r\n\r\nCount_Get with StreamKind=Stream_Audio\r\n";
        ToDisplay += MI.CountGet(StreamKind.Audio).ToString();

        ToDisplay += "\r\n\r\nGet with Stream=General and Parameter='AudioCount'\r\n";
        ToDisplay += MI.Get(StreamKind.General, 0, "AudioCount");

        ToDisplay += "\r\n\r\nGet with Stream=Audio and Parameter='StreamCount'\r\n";
        ToDisplay += MI.Get(StreamKind.Audio, 0, "StreamCount");

        ToDisplay += "\r\n\r\nClose\r\n";
        MI.Close();

        richTextBox1.Text = ToDisplay;
    }
}

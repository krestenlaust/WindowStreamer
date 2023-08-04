using System;
using System.Net;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommonApp;
using LabelSink;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using WindowStreamer.Client;
using WindowStreamer.Image;
using WindowStreamer.Image.Windows;

namespace ClientApp;

/// <summary>
/// Main Forms-class associated with displaying the video stream.
/// </summary>
[SupportedOSPlatform("windows")]
public partial class WindowDisplay : Form
{
    static readonly int DefaultMetastreamPort = 10063;
    static readonly int FramerateCap = 30;

    readonly IServiceProvider serviceProvider;
    System.Drawing.Size formToPanelSize;
    System.Drawing.Size videoResolution;

    WindowClient windowClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowDisplay"/> class.
    /// </summary>
    public WindowDisplay()
    {
        InitializeComponent();

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Logger(lc => lc
                .MinimumLevel.Information()
                .WriteTo.Console(Serilog.Events.LogEventLevel.Debug)
                .WriteTo.ToolStripLabel(toolStripStatusLabelLatest))
            .WriteTo.File("log-client.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        serviceProvider = new ServiceCollection()
            .AddTransient<IImageFactory, NativeImageFactory>()
            .BuildServiceProvider();
    }

    async void WindowDisplay_LoadAsync(object sender, EventArgs e)
    {
        formToPanelSize = System.Drawing.Size.Subtract(Size, displayArea.Size);

        // Update UI
        ResizeToFit();
        ToggleDragableBorder(!toolStripButtonResizeToFit.Checked);
        toolStripStatusLabelResolution.Text = string.Empty;

        await HandleCommandlineArgumentsAsync(Environment.GetCommandLineArgs());
    }

    async Task HandleCommandlineArgumentsAsync(string[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            if (!args[i].StartsWith("-"))
            {
                continue;
            }

            switch (args[i])
            {
                case "--connect":
                case "-c":
                    if (args.Length == i + 1)
                    {
                        // No parameter given.
                        continue;
                    }

                    string parameter = args[i + 1];

                    var splittedParameter = parameter.Split(':');

                    if (!IPAddress.TryParse(splittedParameter[0], out IPAddress address))
                    {
                        // Invalid parameter
                        continue;
                    }

                    if (splittedParameter.Length == 1 || !int.TryParse(splittedParameter[1], out int targetPort))
                    {
                        targetPort = DefaultMetastreamPort;
                    }

                    await StartConnectionToServer(address, targetPort).ConfigureAwait(false);
                    break;
            }
        }
    }

    async void toolStripButtonConnect_ClickAsync(object sender, EventArgs e)
    {
        using var connectDialog = new ConnectWindow(IPAddress.Loopback, DefaultMetastreamPort);

        if (connectDialog.ShowDialog() == DialogResult.OK)
        {
            await StartConnectionToServer(connectDialog.TargetIPAddress, connectDialog.TargetPort);
        }
    }

    async Task StartConnectionToServer(IPAddress address, int targetPort)
    {
        if (windowClient is not null)
        {
            windowClient.ResolutionChanged -= WindowClient_ResolutionChanged;
            windowClient.NewFrame -= WindowClient_VideoframeRecieved;
            windowClient.ConnectionClosed -= WindowClient_ConnectionClosed;

            windowClient.Dispose();
        }

        windowClient = new WindowClient(
            address,
            targetPort,
            FramerateCap,
            serviceProvider.GetRequiredService<IImageFactory>());

        windowClient.ResolutionChanged += WindowClient_ResolutionChanged;
        windowClient.NewFrame += WindowClient_VideoframeRecieved;
        windowClient.ConnectionClosed += WindowClient_ConnectionClosed;

        await windowClient.ConnectToServerAsync().ConfigureAwait(false);
    }

    private void WindowClient_ConnectionClosed()
    {
        Log.Information("Server disconnected");
    }

    void WindowClient_VideoframeRecieved(IImage bitmap)
    {
        if (!displayArea.IsHandleCreated)
        {
            return;
        }

        displayArea.Invoke((MethodInvoker)delegate
        {
            displayArea.Image = (NativeImage)bitmap;
        });
    }

    void WindowClient_ResolutionChanged(WindowStreamer.Image.Size obj)
    {
        videoResolution = new System.Drawing.Size(obj.Width, obj.Height);

        if (toolStripButtonResizeToFit.Checked)
        {
            Invoke((MethodInvoker)delegate
            {
                ResizeToFit();
                toolStripStatusLabelResolution.Text = $"{videoResolution.Width}x{videoResolution.Height}";
            });
        }
    }

    void toolStripButtonResizeToFit_Click(object sender, EventArgs e)
    {
        Log.Debug($"Fit to content: {toolStripButtonResizeToFit.Checked}");

        if (toolStripButtonResizeToFit.Checked)
        {
            ResizeToFit();
        }

        ToggleDragableBorder(!toolStripButtonResizeToFit.Checked);
    }

    void ToggleDragableBorder(bool dragable)
    {
        if (dragable)
        {
            FormBorderStyle = FormBorderStyle.Sizable;
            statusStripFooter.SizingGrip = true;
            MaximizeBox = true;
        }
        else
        {
            FormBorderStyle = FormBorderStyle.FixedSingle;
            statusStripFooter.SizingGrip = false;
            MaximizeBox = false;

            if (WindowState == FormWindowState.Maximized)
            {
                WindowState = FormWindowState.Normal;
            }
        }
    }

    void ResizeToFit() => ResizeDisplayArea(videoResolution);

    void ResizeDisplayArea(System.Drawing.Size size) => Size = System.Drawing.Size.Add(formToPanelSize, size);

    private void WindowDisplay_FormClosed(object sender, FormClosedEventArgs e)
    {
        Log.Information("Application closing...");
        Log.CloseAndFlush();
    }

    private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
    {
        new AboutBoxMain().ShowDialog();
    }

    private void helpToolStripMenuItem_Click(object sender, EventArgs e)
    {
        Helper.OpenUrl(ProjectProperties.GithubUrl);
    }
}

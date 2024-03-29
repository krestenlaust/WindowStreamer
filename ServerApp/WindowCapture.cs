﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommonApp;
using LabelSink;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using WindowStreamer.Image.Windows;
using WindowStreamer.Server;

namespace ServerApp;

[SupportedOSPlatform("windows")]
public partial class WindowCapture : Form
{
    static readonly int DefaultMetastreamPort = 10063;
    static readonly Color TransparencyKeyColor = Color.Orange;

    readonly IServiceProvider serviceProvider;
    bool fullscreen;
    Size videoResolution;
    Size lastResolution; // TODO: Not sure what this field keeps track of.
    WindowServer server;

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowCapture"/> class.
    /// </summary>
    public WindowCapture()
    {
        InitializeComponent();

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Logger(lc => lc
                .MinimumLevel.Information()
                .WriteTo.Console(Serilog.Events.LogEventLevel.Debug)
                .WriteTo.ToolStripLabel(toolStripStatusLabelLatest))
            .WriteTo.File("log-server.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        serviceProvider = new ServiceCollection()
            .AddTransient<IGetCaptureArea>(x => new GetCaptureAreaFromControls(this, captureArea, toolStripHeader))
            .AddTransient<IConnectionHandler, ConnectionHandler>()
            .AddTransient<IScreenshotQuery, ScreenshotGrabber>()
            .BuildServiceProvider();
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == 0x0112) // WM_SYSCOMMAND
        {
            if (m.WParam == new IntPtr(0xF030)) // Maximize event - SC_MAXIMIZE from Winuser.h
            {
                // TODO: Not sure what to do here.
                fullscreen = true;
                lastResolution = Size;
                UpdateResolutionVariables();
            }
            else if (m.WParam == new IntPtr(0xF120))
            {
                fullscreen = false;
                Size = new Size(lastResolution.Width, lastResolution.Height);
                UpdateResolutionVariables();
            }
        }

        base.WndProc(ref m);
    }

    async void WindowCapture_LoadAsync(object sender, EventArgs e)
    {
        TransparencyKey = TransparencyKeyColor;
        captureArea.BackColor = TransparencyKeyColor;
        UpdateResolutionVariables();
        toolStripTextBoxTargetPort.Text = DefaultMetastreamPort.ToString();

        // Update UI
        ToggleActionStartStopButtons(false);

        // Handle command-line arguments
        await HandleCommandlineArgumentsAsync(Environment.GetCommandLineArgs());
    }

    async Task HandleCommandlineArgumentsAsync(string[] args)
    {
        ICollection<Task> tasks = new List<Task>();

        for (int i = 0; i < args.Length; i++)
        {
            if (!args[i].StartsWith("-"))
            {
                continue;
            }

            switch (args[i])
            {
                case "--listen":
                case "-l":
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

                    tasks.Add(Task.Run(() => StartServerAsync(address, targetPort)));
                    break;
                default:
                    break;
            }
        }

        await Task.WhenAll(tasks);
    }

    void WindowCapture_Resize(object sender, EventArgs e) => UpdateResolutionVariables();

    void toolStripButtonOptions_Click(object sender, EventArgs e) => new Options().ShowDialog();

    async void toolStripButtonActionStart_ClickAsync(object sender, EventArgs e)
    {
        IPAddress acceptedAddress;
        if (toolStripTextBoxAcceptableHost.Text == string.Empty)
        {
            acceptedAddress = IPAddress.Any;
        }
        else if (!IPAddress.TryParse(toolStripTextBoxAcceptableHost.Text, out acceptedAddress))
        {
            MessageBox.Show("IP not valid", "Error");
            return;
        }

        if (!int.TryParse(toolStripTextBoxTargetPort.Text, out int targetPort))
        {
            targetPort = DefaultMetastreamPort;
        }

        await StartServerAsync(acceptedAddress, targetPort);
    }

    void toolStripButtonActionStop_Click(object sender, EventArgs e)
    {
        server?.Dispose();
        server = null;
        ToggleActionStartStopButtons(false);

        Log.Information("Stopped server");
    }

    void ToggleActionStartStopButtons(bool serverRunning)
    {
        toolStripButtonActionStart.Visible = !serverRunning;
        toolStripButtonActionStop.Visible = serverRunning;
    }

    async Task StartServerAsync(IPAddress bindAddress, int? port)
    {
        if (port is null)
        {
            port = DefaultMetastreamPort;
        }

        if (server is not null)
        {
            server.ConnectionClosed -= WindowServer_ConnectionClosed;
            server.Dispose();
        }

        ToggleActionStartStopButtons(true);

        server = new WindowServer(
            bindAddress,
            port.Value,
            videoResolution.ToImageSize(),
            serviceProvider.GetRequiredService<IScreenshotQuery>(),
            serviceProvider.GetRequiredService<IConnectionHandler>(),
            serviceProvider.GetRequiredService<IGetCaptureArea>());

        server.ConnectionClosed += WindowServer_ConnectionClosed;

        try
        {
            await server.StartServerAsync().ConfigureAwait(false);
        }
        catch (SocketException ex)
        {
            Log.Information($"Couldn't start server: {ex}");
            ToggleActionStartStopButtons(false);
        }
    }

    private void WindowServer_ConnectionClosed()
    {
        Log.Information("Client disconnected");
        ToggleActionStartStopButtons(false);
    }

    void UpdateResolutionVariables()
    {
        if (fullscreen)
        {
            videoResolution = Screen.FromControl(this).Bounds.Size;
        }
        else
        {
            videoResolution = captureArea.Size;
        }

        toolStripStatusLabelResolution.Text = $"{videoResolution.Width}x{videoResolution.Height}";
        server?.UpdateResolution(videoResolution.ToImageSize());
    }

    private void WindowCapture_FormClosed(object sender, FormClosedEventArgs e)
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
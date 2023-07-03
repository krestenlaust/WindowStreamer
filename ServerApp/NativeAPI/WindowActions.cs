using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ServerApp.NativeAPI;

internal static class WindowActions
{
    [DllImport("user32.dll")]
    static extern bool ClientToScreen(IntPtr hWnd, ref Point lpPoint);
}
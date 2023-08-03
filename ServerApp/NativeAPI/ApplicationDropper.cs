using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ServerApp.NativeAPI;

// https://social.msdn.microsoft.com/Forums/en-US/bfc75b57-df16-48c6-92af-ea0a34f540ae/how-to-get-the-handle-of-a-window-that-i-click?forum=csharplanguage
internal static class ApplicationDropper
{
    const int WH_MOUSE_LL = 14;

    static readonly LowLevelMouseProc _proc = HookCallback;
    static readonly IntPtr _hookID;
    static IntPtr hHook;

    delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

    enum MouseMessages
    {
        WM_LBUTTONDOWN = 0x0201,
        WM_LBUTTONUP = 0x0202,
        WM_MOUSEMOVE = 0x0200,
        WM_MOUSEWHEEL = 0x020A,
        WM_RBUTTONDOWN = 0x0204,
        WM_RBUTTONUP = 0x0205,
    }

    public static void AddGlobalClickHook()
    {
        if (hHook == IntPtr.Zero)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                hHook = SetWindowsHookEx(
                    WH_MOUSE_LL,
                    _proc,
                    GetModuleHandle(curModule.ModuleName),
                    0);
            }
        }
    }

    static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && (MouseMessages)wParam == MouseMessages.WM_LBUTTONDOWN)
        {
            // The application runs to here when you click on the window whose handle you  want to get
            POINT cusorPoint;
            bool ret = GetCursorPos(out cusorPoint);

            // cusorPoint contains your cusor’s position when you click on the window
            // Then use cusorPoint to get the handle of the window you clicked
            IntPtr winHandle = WindowFromPoint(cusorPoint);

            MessageBox.Show(winHandle.ToString());

            // Because the hook may occupy much memory, so remember to uninstall the hook after
            // you finish your work, and that is what the following code does.
            UnhookWindowsHookEx(hHook);
            hHook = IntPtr.Zero;

            // Here I do not use the GetActiveWindow(). Let's call the window you clicked "DesWindow" and explain my reason.
            // I think the hook intercepts the mouse click message before the mouse click message delivered to the DesWindow's
            // message queue. The application came to this function before the DesWindow became the active window, so the handle
            // abtained from calling GetActiveWindow() here is not the DesWindow's handle, I did some tests, and What I got is always
            // the Form's handle, but not the DesWindow's handle. You can do some test too.

            // IntPtr handle = GetActiveWindow();
        }

        return CallNextHookEx(_hookID, nCode, wParam, lParam);
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool GetCursorPos(out POINT lpPoint);

    [DllImport("user32.dll")]
    static extern IntPtr WindowFromPoint(POINT point);

    [StructLayout(LayoutKind.Sequential)]
    struct POINT
    {
        public int x;
        public int y;
    }
}

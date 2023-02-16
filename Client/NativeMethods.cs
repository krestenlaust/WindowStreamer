using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Client
{
    public static class NativeMethods
    {
        //Click through
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;

        public static void PerformClick(uint cursorX, uint cursorY)
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, cursorX, cursorY, 0, 0);
        }
    }
}

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Shared;

namespace Server
{

    public static class WindowActions
    {
        public static void ClickOnPoint(IntPtr handle, Point point, int mouseButton = 0)
        {
            if (handle == IntPtr.Zero)
            {
                Console.WriteLine("Handle not attached");
                return;
            }

            Point cursorPosition = Cursor.Position;

            NativeMethods.ClientToScreen(handle, ref point);

            Cursor.Position = new Point(point.X, point.Y);

            NativeMethods.INPUT inputMouseDown = new NativeMethods.INPUT();
            inputMouseDown.Type = 0; /// input type mouse
            inputMouseDown.Data.Mouse.Flags = 0x0002; /// left button down

            NativeMethods.INPUT inputMouseUp = new NativeMethods.INPUT();
            inputMouseUp.Type = 0; /// input type mouse
            inputMouseUp.Data.Mouse.Flags = 0x0004; /// left button up

            var inputs = new NativeMethods.INPUT[] { inputMouseDown, inputMouseUp };
            NativeMethods.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(NativeMethods.INPUT)));

            // return mouse
            Cursor.Position = cursorPosition;
        }

        public static void InputKey(IntPtr handle, Keys key)
        {
        }
    }
}
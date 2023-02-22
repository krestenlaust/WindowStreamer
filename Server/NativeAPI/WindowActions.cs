using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Server.NativeAPI
{
    internal static class WindowActions
    {
        [DllImport("user32.dll")]
        static extern bool ClientToScreen(IntPtr hWnd, ref Point lpPoint);

        [DllImport("user32.dll")]
        static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs, int cbSize);

        public static void ClickOnPoint(IntPtr handle, Point point, int mouseButton = 0)
        {
            if (handle == IntPtr.Zero)
            {
                Console.WriteLine("Handle not attached");
                return;
            }

            Point cursorPosition = Cursor.Position;

            ClientToScreen(handle, ref point);

            Cursor.Position = new Point(point.X, point.Y);

            INPUT inputMouseDown = default;
            inputMouseDown.Type = 0; // input type mouse
            inputMouseDown.Data.Mouse.Flags = 0x0002; // left button down

            INPUT inputMouseUp = default;
            inputMouseUp.Type = 0; // input type mouse
            inputMouseUp.Data.Mouse.Flags = 0x0004; // left button up

            var inputs = new INPUT[] { inputMouseDown, inputMouseUp };
            SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));

            // return mouse
            Cursor.Position = cursorPosition;
        }

        public static void InputKey(IntPtr handle, Keys key)
        {
        }

        public struct INPUT
        {
            public int Type;
            public MOUSEKEYBDHARDWAREINPUT Data;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct MOUSEKEYBDHARDWAREINPUT
        {
            [FieldOffset(0)]
            public MOUSEINPUT Mouse;
        }

        public struct MOUSEINPUT
        {
            public int X;
            public int Y;
            public uint MouseData;
            public uint Flags;
            public uint Time;
            public IntPtr ExtraInfo;
        }
    }
}
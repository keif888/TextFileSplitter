using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Martin.SQLServer.Dts
{
    public static class DrawingControl
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass"), System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, Int32 wMsg, IntPtr wParam, IntPtr lParam);

        private const int WM_SETREDRAW = 11;

        public static void SuspendDrawing(Control parent)
        {
            SendMessage(parent.Handle, WM_SETREDRAW, (IntPtr) 0, IntPtr.Zero);
        }

        public static void ResumeDrawing(Control parent)
        {
            SendMessage(parent.Handle, WM_SETREDRAW, (IntPtr) 1, IntPtr.Zero);
            parent.Refresh();
        }
    }
}

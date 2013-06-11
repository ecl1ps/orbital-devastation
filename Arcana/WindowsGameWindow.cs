using System;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Graphics;

namespace Arcane.Xna.Presentation
{
    class WindowsGameWindow
    {

        private static string InvalidScreenAdapter = "The adapter does not map to a valid monitor screen.";

        internal static string DeviceNameFromScreen(Screen screen)
        {
            string deviceName = screen.DeviceName;
            int index = screen.DeviceName.IndexOf('\0');
            if (index != -1)
            {
                deviceName = screen.DeviceName.Substring(0, index);
            }
            return deviceName;
        }

        internal static Screen ScreenFromAdapter(GraphicsAdapter adapter)
        {
            foreach (Screen screen in Screen.AllScreens)
            {
                if (DeviceNameFromScreen(screen) == adapter.DeviceName)
                {
                    return screen;
                }
            }
            throw new ArgumentException(InvalidScreenAdapter, "adapter");
        }



        internal static Screen ScreenFromHandle(IntPtr windowHandle)
        {
            NativeMethods.RECT rect;
            int num = 0;
            Screen screen = null;
            NativeMethods.GetWindowRect(windowHandle, out rect);
            System.Drawing.Rectangle rectangle = new System.Drawing.Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
            foreach (Screen screen2 in Screen.AllScreens)
            {
                System.Drawing.Rectangle rectangle2 = rectangle;
                rectangle2.Intersect(screen2.Bounds);
                int num2 = rectangle2.Width * rectangle2.Height;
                if (num2 > num)
                {
                    num = num2;
                    screen = screen2;
                }
            }
            if (screen == null)
            {
                screen = Screen.AllScreens[0];
            }
            return screen;
        }

    }
}

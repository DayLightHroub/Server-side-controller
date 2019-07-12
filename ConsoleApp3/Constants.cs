

using System;
using System.Runtime.InteropServices;

namespace ConsoleApp3
{
    static class Constants
    {
        private static int windowHeight;

        public const int TIME_OUT_TIMER_LEAGUE = 40000;

        public const int TIME_OUT_TIMER_IMAGE = 5000;

        public const int PORT_NO = 8888;

        /*for 1600 height resolution*/
        //point target color poition
        public const int POINT_X_1600 = 1401;
        public const int POINT_Y_1600 = 101;

        //password text filed posiiton
        public const int PASS_X_1600 = 1392;
        public const int PASS_Y_1600 = 307;

        //Color at target point
        public const int RED_1600 = 19;
        public const int GREEN_1600 = 30;
        public const int BLUE_1600 = 27;


        /*for 1280 height resolution*/
        //point target color poition
        public const int POINT_X_1280 = 1126;
        public const int POINT_Y_1280 = 81;

        //password text filed posiiton
        public const int PASS_X_1280 = 1113;
        public const int PASS_Y_1280 = 253;

        //Color at target point
        public const int RED_1280 = 31;
        public const int GREEN_1280 = 23;
        public const int BLUE_1280 = 13;

        /*for 1024 height resolution*/
        //point target color poition
        public const int POINT_X_1024 = 902;
        public const int POINT_Y_1024 = 65;

        //password text filed posiiton
        public const int PASS_X_1024 = 896;
        public const int PASS_Y_1024 = 194;

        //Color at target point
        public const int RED_1024 = 96;
        public const int GREEN_1024 = 70;
        public const int BLUE_1024 = 26;


        //filename
        public const String FILE_NAME_LOL = "config.txt";

        //RECT to determine the window coordinates, used to find the origin of window
        private static RECT rct = new RECT();

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);
        [StructLayout(LayoutKind.Sequential)]


        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }


        public static void setOrigins(IntPtr windowHandle)
        {
            GetWindowRect(windowHandle, ref rct);
            windowHeight = rct.Right - rct.Left;
            //          Console.WriteLine(getWindowHeight());

        }

        public static int getXORIGIN()
        {
            return rct.Left;
        }

        public static int getYORIGIN()
        {
            return rct.Top;
        }

        public static int getWindowHeight()
        {
            return windowHeight;
        }





    }
}

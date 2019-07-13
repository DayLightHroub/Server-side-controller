using System;

using System.Drawing;
using System.Drawing.Imaging;

using System.Runtime.InteropServices;

using System.Threading;


namespace ConsoleApp3
{
    class ScreenDetection
    {
        //The color at poition X and Y
        private static int RED;
        private static int GREEN;
        private static int BLUE;
        //Position X AND Y
        private static int X;//1561;
        private static int Y;//171;



        static Bitmap screenPixel = new Bitmap(1, 1, PixelFormat.Format32bppArgb);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int BitBlt(IntPtr hDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);


        static private Color getColorAt(Point location)
        {
            using (Graphics gdest = Graphics.FromImage(screenPixel))
            {
                using (Graphics gsrc = Graphics.FromHwnd(IntPtr.Zero))
                {
                    IntPtr hSrcDC = gsrc.GetHdc();
                    IntPtr hDC = gdest.GetHdc();
                    int retval = BitBlt(hDC, 0, 0, 1, 1, hSrcDC, location.X, location.Y, (int)CopyPixelOperation.SourceCopy);
                    gdest.ReleaseHdc();
                    gsrc.ReleaseHdc();
                }
            }

            return screenPixel.GetPixel(0, 0);
        }

        static public bool holdtillDetected(IntPtr windowHandle)
        {
            //determine x and y position of window, rect.left for x, rect.top for y
            Constants.setOrigins(windowHandle);
            if (Constants.getWindowHeight() == 1600)
            {
                
                X = Constants.POINT_X_1600;
                Y = Constants.POINT_Y_1600;
                RED = Constants.RED_1600;
                BLUE = Constants.BLUE_1600;
                GREEN = Constants.GREEN_1600;
            }
            else if (Constants.getWindowHeight() == 1280)
            {
                X = Constants.POINT_X_1280;
                Y = Constants.POINT_Y_1280;
                RED = Constants.RED_1280;
                BLUE = Constants.BLUE_1280;
                GREEN = Constants.GREEN_1280;
            }
            else
            {
                X = Constants.POINT_X_1024;
                Y = Constants.POINT_Y_1024;
                RED = Constants.RED_1024;
                BLUE = Constants.BLUE_1024;
                GREEN = Constants.GREEN_1024;
            }
            //get the color of the target pixel
            Point location = new Point(X + Constants.getXORIGIN(), Y + Constants.getYORIGIN());
            //Console.WriteLine(location.ToString());
            Color targetC = Color.FromArgb(RED, GREEN, BLUE);
            while (true)
            {
                //TEST
                
                if (Program.exitThread)
                    return false;

                //TEST
                //if color of target pixel equals the color of the precalculated pixel return
                if (getColorAt(location) == targetC)
                    return true;

                Constants.setOrigins(windowHandle);
                location.X = X + Constants.getXORIGIN();
                location.Y = Y + Constants.getYORIGIN();
                Thread.Sleep(100);
            }
        }



    }
}

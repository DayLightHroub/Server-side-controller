using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Sockets;

using System.Timers;
using System.Windows.Forms;

namespace ConsoleApp3
{
    class ImageSnap
    {
        private System.Timers.Timer exitTimer;
        private TcpListener listener;
        
        public ImageSnap()
        {

            setTimer();

            //initConnection

            //---listen at the specified IP and port no.---

            listener = new TcpListener(IPAddress.Any, Constants.PORT_NO);
           // Console.WriteLine("Waiting for client...");
            listener.Start();
            
            //---incoming client connected---
            
            TcpClient client = null;
            try{
                client = listener.AcceptTcpClient();
            } catch(SocketException e)
            {
                //Console.WriteLine(e);
                listener.Stop();
               // Console.WriteLine("no client, closing");
                return;
            }

          
            //---get the incoming data through a network stream---
            NetworkStream nwStream = client.GetStream();
           // Console.WriteLine("Client connected");

            //send to client message


            byte[] image = getImage();

            nwStream.Write(image, 0, image.Length);
            client.Close();
            listener.Stop();
            finishTimer();
            //Console.WriteLine("done...");
        }

        private byte[] ImageToByte2(Image img)
        {
            using (var stream = new MemoryStream())
            {
                img.Save(stream, ImageFormat.Png);
                return stream.ToArray();
            }
        }

        private byte[] getImage()
        {
            //Console.WriteLine("taking image");
            //Create a new bitmap.
            var bmpScreenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width,
                                           Screen.PrimaryScreen.Bounds.Height,
                                           PixelFormat.Format32bppArgb);

            // Create a graphics object from the bitmap.
            var gfxScreenshot = Graphics.FromImage(bmpScreenshot);

            // Take the screenshot from the upper left corner to the right bottom corner.
            gfxScreenshot.CopyFromScreen(Screen.PrimaryScreen.Bounds.X,
                                        Screen.PrimaryScreen.Bounds.Y,
                                        0,
                                        0,
                                        Screen.PrimaryScreen.Bounds.Size,
                                        CopyPixelOperation.SourceCopy);



            return ImageToByte2(bmpScreenshot);
        }

        private void setTimer()
        {
            // Create a timer with a two second interval.
            exitTimer = new System.Timers.Timer(Constants.TIME_OUT_TIMER_IMAGE);
            // Hook up the Elapsed event for the timer. 
            exitTimer.Elapsed += OnTimedEvent;
            exitTimer.AutoReset = false;
            exitTimer.Enabled = true;
        }

        private void finishTimer()
        {
            exitTimer.Stop();
            exitTimer.Dispose();
            //Console.WriteLine("timer closed");

        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {

            //Console.WriteLine("image timer finished");
            listener.Server.Close();
        }
    }
}

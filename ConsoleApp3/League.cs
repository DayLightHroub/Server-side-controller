using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Timers;
using System.Windows.Forms;

namespace ConsoleApp3
{
    
    class League
    {
        
        private System.Timers.Timer exitTimer;

        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;

        //password text position on league window
        private int XPOS;
        private int YPOS;


        


        [DllImport("user32.dll")]
        public static extern int SetForegroundWindow(IntPtr hWnd);

        public League()
        {
            Program.wake();
            //settimer
            setTimer();

            Write.initFile();
            Read.initReading();
            Process.Start(Read.getGameLocation());
            intiProcedure(Read.getPassword());
        }

        void intiProcedure(string password)
        {
           // Console.WriteLine("LOLPASSWORD: lol thread started");
            Process[] processlist;
            processlist = Process.GetProcessesByName("LeagueClientUx");
            if (processlist.Length > 0 && !String.IsNullOrEmpty(processlist[0].MainWindowTitle))
            {

                goto done;
            }
            //Console.WriteLine("Searching for league window");
            Program.sendToClient("searchingL");
            while (processlist.Length == 0 || String.IsNullOrEmpty(processlist[0].MainWindowTitle))
            {
                //TEST
                if (Program.exitThread)
                {
                    finishTimer();
                    //Console.WriteLine("exittrhead stop");
                    Program.counter--;
                    return;
                }
                //TEST
                processlist = Process.GetProcessesByName("LeagueClientUx");
                Thread.Sleep(1000);
            }


            //Console.WriteLine("Preparing to login");
            Program.sendToClient("prepL");

            done:
            SetForegroundWindow(processlist[0].MainWindowHandle);

            if (!ScreenDetection.holdtillDetected(processlist[0].MainWindowHandle))
            {
                finishTimer();
                //Console.WriteLine("exittrhead stop");
                Program.counter--;
                return;
            }
            
            Program.sendToClient("sdetcted");
            //Thread.Sleep(timer);

            //set cursor and click with delay
            if (Constants.getWindowHeight() == 1600)
            {
                XPOS = Constants.PASS_X_1600;
                YPOS = Constants.PASS_Y_1600;
            }
            else if (Constants.getWindowHeight() == 1280)
            {
                XPOS = Constants.PASS_X_1280;
                YPOS = Constants.PASS_Y_1280;
            }
            else
            {
                XPOS = Constants.PASS_X_1024;
                YPOS = Constants.PASS_Y_1024;
            }
            setCursorAndClick();
            Thread.Sleep(300);
            setCursorAndClick();
            SendKeys.SendWait(password);
            Thread.Sleep(100);
            SendKeys.SendWait("~");

            //TEST
            Program.counter--;

            //TEST

            finishTimer();



        }




        void setCursorAndClick()
        {
            Point oldLocation = Cursor.Position;
            Cursor.Position = new Point(XPOS + Constants.getXORIGIN(), YPOS + Constants.getYORIGIN());
            Program.mouse_event(MOUSEEVENTF_LEFTDOWN, XPOS, YPOS, 0, 0);
            Program.mouse_event(MOUSEEVENTF_LEFTUP, XPOS, YPOS, 0, 0);
            Cursor.Position = oldLocation;
        }

        private void setTimer()
        {
            // Create a timer with a two second interval.
            exitTimer = new System.Timers.Timer(Constants.TIME_OUT_TIMER_LEAGUE);
            // Hook up the Elapsed event for the timer. 
            exitTimer.Elapsed += OnTimedEvent;
            exitTimer.AutoReset = false;
            exitTimer.Enabled = true;
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            
            //Console.WriteLine("TIMER: Finishing thread");
            Program.exitThread = true;

        }

        private void finishTimer()
        {
            //Console.WriteLine("TIMER: FINISHED");
            exitTimer.Stop();
            exitTimer.Dispose();
        }
    }
}

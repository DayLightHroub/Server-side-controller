
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ConsoleApp3
{
    class Program
    {
        //Used for handling even on console window closing
        private delegate bool ConsoleCtrlHandlerDelegate(int sig);

        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(ConsoleCtrlHandlerDelegate handler, bool add);
       
        static ConsoleCtrlHandlerDelegate _consoleCtrlHandler;

        public static IPAddress senderAddress;

        static IPEndPoint endpoint;

        static  Socket sckt;

        static private IPAddress localIp = GetLocalIPAddress();

        static private Thread lolThread;
        static private Thread imageThread;

        static public bool exitThread = false;

        static public int counter = 1;

        static public bool passSent = false;

        static public string lolPassword = "";

        //Used to wake the monitor
        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        //usedw to turn off the monitor
        [DllImport("user32.dll")]
        private static extern int SendMessage(int hWnd, int hMsg, int wParam, int lParam);

        //constants for moving the mouse to wake the monitor and turning off the moniotr
        private const int MOUSEEVENTF_MOVE = 0x0001;
        private const int MonitorStateOff = 2;


        static void Main(string[] args)
        {
            

            //Console.WriteLine("Listening");

            /* TODO
             * 
             * 

             * 4- this wasn't yet done in client side, but add a new command which will capture a screenshot of desktop, send it to client.
             */
            setOnSetStartup();
            sckt = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            
            
            //close the socket when applicaiton exit
            _consoleCtrlHandler += s =>
            {
                
                sendBroadCast("shut");
                sckt.Close();
                return false;
            };

            SetConsoleCtrlHandler(_consoleCtrlHandler, true);

            byte[] data = new byte[1024];
           
            endpoint = new IPEndPoint(IPAddress.Any, Constants.PORT_NO);
            
            sckt.Bind(endpoint);

            //IPEndPoint senderEndPoint;

            EndPoint remote = (EndPoint)endpoint;
            
            sendBroadCast("on");
            while (true)
            {
               
                //receive data from sender
                int recv = sckt.ReceiveFrom(data, ref remote);
                                   
                string message = Encoding.ASCII.GetString(data, 0, recv);

                //Get sender ip address
                //Console.WriteLine(senderAddress.ToString());
                senderAddress = ((IPEndPoint)remote).Address;

                //Console.WriteLine(message);
                if (senderAddress.Equals(localIp))
                    continue;

                //Console.WriteLine(message);

                if (message.Equals("lol"))
                {
                    if (lolThread != null && lolThread.IsAlive)
                    {
                        //notify client
                        sendToClient("tworking");
                        //Console.WriteLine("start lol already working..!");
                        continue;

                    }

                    startLol();
                }
                else if (message.Equals("stoplol"))
                {
                    exitThread = true;
                    //TEST
                    counter++;
                    //TEST
                    new Thread(new ThreadStart(startLolWithDelay)).Start();

                }
                else if (message.Equals("stoplolC"))
                {
                    exitThread = true;
                    //TEST
                    counter++;
                    //TEST
                    new Thread(new ThreadStart(startLolWithDelayC)).Start();

                }
                else if (message.Equals("clol"))
                {
                    exitThread = true;
                    new Thread(new ParameterizedThreadStart(exitProgram)).Start("LeagueClient");
                }
                else if (message.Equals("chrome"))
                    Process.Start(@"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe");
                else if (message.Equals("cchrome"))
                    new Thread(new ParameterizedThreadStart(exitProgram)).Start("chrome");
                else if (message.Equals("shut"))
                {
                    sendToClient("shut");
                    Process.Start("shutdown", "/s /t 0");
                }
                else if (message.Equals("restart")) {
                    sendToClient("shut");
                    Process.Start("shutdown", "/r /t 0");
                    }
                else if (message.Equals("image"))
                {
                    startSendingProcess();
                }

                else if (message.Equals("moff"))
                {
                    //This will turn off the monitor
                    SendMessage(0xFFFF, 0x112, 0xF170, MonitorStateOff);
                }

                else if (message.Equals("mon"))
                {
                    wake();
                }

                else if(message.Length > 5 && (message.Substring(0, 5).Equals("cpass")))
                {
                    //take the password from the message...
                    lolPassword = message.Substring(5, message.Length - 5);
                    new Thread(new ParameterizedThreadStart(changeLolPw)).Start(lolPassword);
                }

                else if (message.Length > 4 && (message.Substring(0, 4).Equals("pass")))
                {
                    //take the password from the message...
                    lolPassword = message.Substring(4, message.Length - 4);
                    //Console.WriteLine("lol pass: " + lolPassword);
                    passSent = true;

                }
                

                sendToClient(message);
                    
                




            }
            
                        

        }

        private static void changeLolPw(object obj)
        {
            Write.initFile();
            Write.reWrite(obj.ToString());
        }

        public static void wake()
        {
            mouse_event(MOUSEEVENTF_MOVE, 0, 1, 0, (int)UIntPtr.Zero);
        }

        private static void startSendingProcess()
        {
            if (imageThread != null && imageThread.IsAlive)
            {
                //notify client
                //Console.WriteLine("image taken still working!");

            }
            else
            {

                imageThread = new Thread(new ThreadStart(takeImageAndSend));
                imageThread.Start();

            }
        }

        private static void takeImageAndSend()
        {
            new ImageSnap();
        }
        private static void startLolWithDelayC()
        {

            exitProgram("LeagueClient");                        
            startLolWithDelay();
        }

        private static void exitProgram(object name)
        {

            foreach (var process in Process.GetProcessesByName((string) name))
            {
                process.Kill();
            }
        }

        private static void startLolWithDelay()
        {
            //Console.WriteLine("LOL: starting lol with delay");
            Thread.Sleep(500);
            //Console.WriteLine("LOL: starting it");
            startLol();
            //Console.WriteLine("LOL: DONE");

            //TEST
            counter--;
            //Console.WriteLine("NUMBER OF THREADS:" + counter);
            //TEST
        }

        private static void startLol()
        {
            //Console.WriteLine("starting procccccess");
            
                exitThread = false;
                lolThread = new Thread(new ThreadStart(launchLeague));
                //TEST
                Program.counter++;
                //TEST
                lolThread.Start();

            
        }

        private static void sendBroadCast(string message)
        {
           
            IPAddress destAddress = IPAddress.Parse("192.168.1.255");

            IPEndPoint destEndPoint = new IPEndPoint(destAddress, Constants.PORT_NO);

            Socket udpSocket = new Socket(destAddress.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

            byte[] dataMessage = Encoding.ASCII.GetBytes(message);
            
            
            udpSocket.SendTo(dataMessage, destEndPoint);

            //Console.WriteLine("sending");
            udpSocket.Shutdown(SocketShutdown.Send);
            udpSocket.Close();

            
        }

       

        private static void launchLeague()
        {
            new League();
        }

        public static void sendToClient(string message)
        {
            //Console.WriteLine(message);
            SendBytesToClient(Encoding.ASCII.GetBytes(message));
        }

        

        public static void SendBytesToClient(byte[] message)
        {
            endpoint.Address = senderAddress;
            sckt.SendTo(message, endpoint);

        }


        private static IPAddress GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip;
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }


        private static void setOnSetStartup()
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            rk.SetValue("Controlling", Application.ExecutablePath);
           

        }
    }
}
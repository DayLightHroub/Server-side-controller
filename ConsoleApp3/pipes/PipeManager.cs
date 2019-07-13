using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp3.pipes
{
    class PipeManager
    {
        public PipeManager()
        {
            new Thread(new ThreadStart(initPipe)).Start();
        }


        private void initPipe()
        {
            while (true)
            {


                NamedPipeServerStream pipeServer =
                  new NamedPipeServerStream("testpipe", PipeDirection.InOut);

                Console.Write("Waiting for client connection...");
                pipeServer.WaitForConnection();

                Console.WriteLine("Client connected.");
                try
                {
                    // Read user input and send that to the client process.
                    StreamWriter sw = new StreamWriter(pipeServer);
                    StreamReader sr = new StreamReader(pipeServer);

                    sw.AutoFlush = true;

                    string message;
                    while ((message = sr.ReadLine()) != null)
                    {
                        Console.WriteLine("[CLIENT]: " + message);
                        analyizCommand(message);
                    }


                    sr.Close();
                    sw.Close();
                }
                // Catch the IOException that is raised if the pipe is broken
                // or disconnected.
                catch (IOException e)
                {
                    Console.WriteLine("ERROR: {0}", e.Message);
                }
                catch (System.ObjectDisposedException e1)
                {
                    Console.WriteLine("client disconncted");
                }

                pipeServer.Close();
            }
        }


        private void analyizCommand(string message)
        {
            switch(message)
            {
               
                case "chrome":
                    Process.Start(@"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe");
                    break;
            }
        }
    }
}

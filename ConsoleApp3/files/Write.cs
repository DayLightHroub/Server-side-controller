

using System;
using System.IO;
using System.Threading;

namespace ConsoleApp3
{
    class Write
    {
        static public string filename;
        public static void initFile()
        {
            string rootLocation = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\ServerControllerConfig";
            Write.filename = rootLocation + "\\" + Constants.FILE_NAME_LOL;

            if (!Directory.Exists(rootLocation))
                Directory.CreateDirectory(rootLocation);

            if (!File.Exists(Write.filename))
                initInstructions();



        }

        public static void  initInstructions()
        {
            string password = "";
            try
            {
               
                Program.sendToClient("npass");
                while (!Program.passSent) Thread.Sleep(100);
                Program.passSent = false;
                password = Program.lolPassword;




                //Pass the filepath and filename to the StreamWriter Constructor
                StreamWriter sw = new StreamWriter(filename);

                //Write a line of text
                sw.WriteLine(";League auto login by DayLight aka Hussein Al-Hroub\n;Version 3\n;Please read the following options and change if any is wrong\n\n\n;Game \"exe\" location, change if this is wrong!" +
                    "\nGame launch location: D:\\Riot Games\\League of Legends\\LeagueClient.exe\n\n;Password required, without it auto login won't work!\nPassword: " + password);


                //Close the file
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }

        }

        public static void reWrite(String newPassword)
        {
            //Console.WriteLine("starting rewriting");

            string[] lines = File.ReadAllLines(Write.filename);

            // Write the new file over the old file.
            using (StreamWriter writer = new StreamWriter(Write.filename))
            {
                
                for (int currentLine = 1; currentLine < lines.Length; ++currentLine)
                {
                   
                   
                     writer.WriteLine(lines[currentLine - 1]);
                    
                }

                writer.WriteLine("Password: " + newPassword);
                
            }

            //Console.WriteLine("done ;)");
        }



    }
}

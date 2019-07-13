using System;

using System.IO;


namespace ConsoleApp3
{
    class Read
    {
        private const int startGReading = 21;

        private const int startPReading = 9;

        private static string gameLocation;

        private static string password;

        public static void initReading()
        {
            gameLocation = @"D:\Riot Games\League of Legends\LeagueClient.exe";

            password = "";

            initValues();
        }

        private static void initValues()
        {
            try
            {
                StreamReader sr = new StreamReader(Write.filename);
                string line = sr.ReadLine();
                while (line != null)
                {
                    int lenghtLine = line.Length;
                    if (lenghtLine > 0)
                    {
                        int i = 0;
                        do
                        {
                            if (line[i] != ' ')
                                goto Safe;
                            i++;
                        } while (i < lenghtLine);
                        goto next;
                        Safe:
                        if (line[i] != ';')
                        {
                            if (i < lenghtLine)
                            {
                                if (line[i] == 'G')
                                {
                                    gameLocation = getValue(startGReading, lenghtLine, line, i);

                                }

                                else if (line[i] == 'P')
                                {
                                    password = getValue(startPReading, lenghtLine, line, i);


                                }
                            }
                        }


                    }
                    next:
                    line = sr.ReadLine();
                }

                //close the file
                sr.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }

        }

        private static string getValue(int startReading, int lenghtLine, string line, int i)
        {
            int j;
            for (j = startReading + i; j < lenghtLine; j++)
                if (line[j] != ' ')
                    break;
            return line.Substring(j);
        }

        public static string getGameLocation()
        {
            return gameLocation;
        }


        public static string getPassword()
        {
            return password;
        }
    }
}

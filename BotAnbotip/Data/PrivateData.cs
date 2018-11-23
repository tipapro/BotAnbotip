using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using BotAnbotip.Data.CustomEnums;

namespace BotAnbotip.Data
{
    class PrivateData
    {
        public static bool Debug = false;


        public static string FileNamePrefix { get; private set; }
        public static string DropboxApiKey { get; private set; }
        public static string MainBotToken { get; private set; }
        public static char MainPrefix { get; private set; }

        public static void Read()
        {
#if DEBUG
            Debug = true;
#endif

            if (Debug)
            {
                using (FileStream testData = new FileStream("B:\\Projects\\testData", FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader reader = new StreamReader(testData))
                    {
                        DropboxApiKey = reader.ReadLine();
                        MainBotToken = reader.ReadLine();
                        MainPrefix = reader.ReadLine().ToCharArray()[0];
                        FileNamePrefix = "Debug";
                    }
                }
            }
            else
            {
                FileNamePrefix = "";
                DropboxApiKey = Environment.GetEnvironmentVariable("DropboxToken");
                MainBotToken = Environment.GetEnvironmentVariable("MainBotToken");
                MainPrefix = '=';
            }
        }

        internal static string GetBotToken(BotType type)
        {
            switch (type)
            {
                case BotType.Main: return MainBotToken;
                default: return "";
            }
        }
    }
}

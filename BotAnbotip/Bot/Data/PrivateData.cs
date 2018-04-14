using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace BotAnbotip.Bot.Data
{
    class PrivateData
    {
        private static string _filename;
        private static string _dropboxApiKey;
        private static string _botToken;
        private static char _prefix;

        public static bool Debug = false;
        

        public static string FileName => _filename;
        public static string DropboxApiKey => _dropboxApiKey;
        public static string BotToken=> _botToken;
        public static char Prefix => _prefix;

        public static void Read()
        {
            if (Debug)
            {
                    using (FileStream testData = new FileStream("B:\\Projects\\testData", FileMode.Open, FileAccess.Read))
                    {
                        using (StreamReader reader = new StreamReader(testData))
                        {
                            _filename = reader.ReadLine();
                            _dropboxApiKey = reader.ReadLine();
                            _botToken = reader.ReadLine();
                            _prefix = reader.ReadLine().ToCharArray()[0];
                        }
                    }

            }
            else
            {
                _filename = "BotAnbotipData";
                _dropboxApiKey = Environment.GetEnvironmentVariable("dropboxToken");
                _botToken = Environment.GetEnvironmentVariable("botToken");
                _prefix = '=';
            }
        }
    }
}

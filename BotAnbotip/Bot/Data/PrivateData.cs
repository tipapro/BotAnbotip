using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace BotAnbotip.Bot.Data
{
    class PrivateData
    {
        private static string _fileNamePrefix;
        private static string _dropboxApiKey;
        private static string _botToken;
        private static char _prefix;

        public static bool Debug = false;


        public static string FileNamePrefix => _fileNamePrefix;
        public static string DropboxApiKey => _dropboxApiKey;
        public static string BotToken => _botToken;
        public static char Prefix => _prefix;

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
                        _dropboxApiKey = reader.ReadLine();
                        _botToken = reader.ReadLine();
                        _prefix = reader.ReadLine().ToCharArray()[0];
                        _fileNamePrefix = "Debug";
                    }
                }
            }
            else
            {
                _fileNamePrefix = "";
                _dropboxApiKey = Environment.GetEnvironmentVariable("dropboxToken");
                _botToken = Environment.GetEnvironmentVariable("botToken");
                _prefix = '=';
            }
        }
    }
}

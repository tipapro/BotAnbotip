using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using BotAnbotip.Bot.Data.CustomEnums;

namespace BotAnbotip.Bot.Data
{
    class PrivateData
    {
        private static string _fileNamePrefix;
        private static string _dropboxApiKey;
        private static string _mainBotToken;
        private static string _auxiliaryBotToken;
        private static char _mainPrefix;
        private static char _auxiliaryPrefix;

        public static bool Debug = false;


        public static string FileNamePrefix => _fileNamePrefix;
        public static string DropboxApiKey => _dropboxApiKey;
        public static string MainBotToken => _mainBotToken;
        public static string AuxiliaryBotToken => _auxiliaryBotToken;
        public static char MainPrefix => _mainPrefix;
        public static char AuxiliaryPrefix => _auxiliaryPrefix;

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
                        _mainBotToken = reader.ReadLine();
                        _auxiliaryBotToken = reader.ReadLine();
                        _mainPrefix = reader.ReadLine().ToCharArray()[0];
                        _auxiliaryPrefix = reader.ReadLine().ToCharArray()[0];
                        _fileNamePrefix = "Debug";
                    }
                }
            }
            else
            {
                _fileNamePrefix = "";
                _dropboxApiKey = Environment.GetEnvironmentVariable("DropboxToken");
                _mainBotToken = Environment.GetEnvironmentVariable("MainBotToken");
                _auxiliaryBotToken = Environment.GetEnvironmentVariable("AuxiliaryBotToken");
                _mainPrefix = '=';
                _auxiliaryPrefix = '}';
            }
        }

        internal static string GetBotToken(BotType type)
        {
            switch (type)
            {
                case BotType.Main: return MainBotToken;
                case BotType.Auxiliary: return AuxiliaryBotToken;
                default: return "";
            }
        }
    }
}

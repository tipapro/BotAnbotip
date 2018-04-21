using System;
using System.Threading.Tasks;
using BotAnbotip.Bot.Data;
using Discord.WebSocket;
using BotAnbotip.Bot.Data.Group;
using BotAnbotip.Bot.Client;
using Discord;

namespace BotAnbotip.Bot.Commands
{
    class HackerChannelCommands
    {
        private static Random random = new Random();
        private const int Length = 10;
        private const int DelayTime = 25;
        private static bool flag;

        public static async Task ChangeStateOfTheHackerChannelAsync(string argument, SocketMessage message = null)
        {
            if (message != null)
            {
                await message.DeleteAsync();
                if (!CommandManager.CheckPermission((IGuildUser)message.Author, RoleIds.Основатель)) return;
            }

            var str = argument.Split(' ');
            if (str.Length > 1)
            {
                DataManager.HackerChannelId = ulong.Parse(str[1]);
                argument = str[0];
                await DataManager.SaveDataAsync(DataManager.HackerChannelId, nameof(DataManager.HackerChannelId));
            }

            if ((argument == "вкл") && (!DataManager.HackerChannelIsRunning))
            {
                DataManager.HackerChannelIsRunning = true;
                await DataManager.SaveDataAsync(DataManager.HackerChannelIsRunning, nameof(DataManager.HackerChannelIsRunning));
                Task.Run(() => RunHackerChannelAsync()).GetAwaiter().GetResult();
            }
            else if (argument == "выкл")
            {
                flag = false;
                DataManager.HackerChannelIsRunning = false;
                await DataManager.SaveDataAsync(DataManager.HackerChannelIsRunning, nameof(DataManager.HackerChannelIsRunning));
            }
        }
        
        private static async void RunHackerChannelAsync()
        {
            try
            {
                if (!BotClient.BotLoaded)
                {
                    return;
                }

                flag = true;

                while (flag)
                {
                    await Task.Delay(DelayTime);
                    await ConstInfo.GroupGuild.GetChannel(DataManager.HackerChannelId).ModifyAsync((channelProperties) =>
                    {
                        channelProperties.Name = GetRandomString(Length);
                    });
                }
                DataManager.HackerChannelIsRunning = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
}

        public static string GetRandomString(int lenght)
        {
            string resultString = "";
            for (int i = 0; i < lenght; i++)
            {
                resultString += GetRandomChar();
            }
            return resultString;
        }

        private static string GetRandomChar()
        {
            return char.ConvertFromUtf32(random.Next(400));
        }
    }
}

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
                DataManager.HackerChannelId.Value = ulong.Parse(str[1]);
                argument = str[0];
                await DataManager.HackerChannelId.SaveAsync();
            }

            if ((argument == "вкл") && (!DataManager.HackerChannelIsRunning.Value))
            {
                DataManager.HackerChannelIsRunning.Value = true;
                await DataManager.HackerChannelIsRunning.SaveAsync();
                Task.Run(() => RunHackerChannelAsync()).GetAwaiter().GetResult();
            }
            else if (argument == "выкл")
            {
                flag = false;
                DataManager.HackerChannelIsRunning.Value = false;
                await DataManager.HackerChannelIsRunning.SaveAsync();
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
                    await ConstInfo.GroupGuild.GetChannel(DataManager.HackerChannelId.Value).ModifyAsync((channelProperties) =>
                    {
                        channelProperties.Name = GetRandomString(Length);
                    });
                }
                DataManager.HackerChannelIsRunning.Value = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (ex.InnerException != null) Console.WriteLine(ex.InnerException.Message);
                Task.Run(() => RunHackerChannelAsync()).GetAwaiter().GetResult();
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

        public static string GetRandomChar()
        {
            return char.ConvertFromUtf32(random.Next(400));
        }
    }
}

using System;
using System.Threading.Tasks;
using BotAnbotip.Bot.Data;
using Discord.WebSocket;

namespace BotAnbotip.Bot.Commands
{
    class ChangeTheChannelCommands
    {
        private static Random random = new Random();
        private const int Length = 10;
        private static bool flag;

        public static async Task SetTheChannelNameAutoChangingAsync(string argument, SocketMessage message = null)
        {           
            if (message != null) await message.DeleteAsync();

            var str = argument.Split(' ');
            if (str.Length > 1)
            {
                DataManager.ChannelNameAutoChangingId = ulong.Parse(str[1]);
                argument = str[0];
            }

            if ((argument == "вкл") && (!DataManager.ChannelNameAutoChangingIsSwitchedOn))
            {
                DataManager.ChannelNameAutoChangingIsSwitchedOn = true;
                await DataManager.SaveDataAsync();
                Task.Run(() => LaunchChannelNameAutoChangingAsync()).GetAwaiter().GetResult();
            }
            else if (argument == "выкл") flag = false;
        }
        
        private static async void LaunchChannelNameAutoChangingAsync()
        {
            flag = true;
            while (flag)
            {
                if (!Info.BotLoaded) continue;
                await Info.GroupGuild.GetChannel(DataManager.ChannelNameAutoChangingId).ModifyAsync((guildChannelProperties) =>
                {
                    guildChannelProperties.Name = RandomString();
                });
            }
            DataManager.ChannelNameAutoChangingIsSwitchedOn = false;
        }

        public static string RandomString()
        {
            string resultString = "";
            for (int i = 0; i < Length; i++)
            {
                resultString += RandomChar();
            }
            return resultString;
        }

        private static string RandomChar()
        {
            return char.ConvertFromUtf32(random.Next(500));
        }
    }
}

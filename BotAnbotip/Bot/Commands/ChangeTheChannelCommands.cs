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

        public static async void SetTheChannelNameAutoChangingAsync(SocketMessage message, string argument)
        {
            await message.DeleteAsync();

            if ((argument == "вкл") && (!Info.ChannelNameAutoChangingIsSwitchedOn))
            {
                Info.ChannelNameAutoChangingIsSwitchedOn = true;
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
                await Info.GroupGuild.GetChannel(427256775193133076).ModifyAsync((guildChannelProperties) =>
                {
                    guildChannelProperties.Name = RandomString();
                });
            }
            Info.ChannelNameAutoChangingIsSwitchedOn = false;
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

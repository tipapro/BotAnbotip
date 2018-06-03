using System;
using System.Threading.Tasks;
using BotAnbotip.Bot.Data;
using Discord.WebSocket;
using BotAnbotip.Bot.Data.Group;
using BotAnbotip.Bot.Clients;
using Discord;
using BotAnbotip.Bot.Commands;
using BotAnbotip.Bot.OtherModules;
using BotAnbotip.Bot.CyclicActions;

namespace BotAnbotip.Bot.Commands
{
    class HackerChannelCommands
    {
        public static async Task ChangeStateOfTheHackerChannelAsync(IMessage message, string argument)
        {
            await message.DeleteAsync();
            if (!CommandManager.CheckPermission((IGuildUser)message.Author, RoleIds.Основатель)) return;

            var str = argument.Split(' ');
            if (str.Length > 1)
            {
                argument = str[0];
                await DataManager.HackerChannelId.SaveAsync(ulong.Parse(str[1]));
            }
            if (argument == "вкл")
            {
                CyclicActionManager.HackerChannelAutoChange.Run();
                await DataManager.HackerChannelIsRunning.SaveAsync(true);
            }
            else if (argument == "выкл")
            {
                CyclicActionManager.HackerChannelAutoChange.Stop();
                await DataManager.HackerChannelIsRunning.SaveAsync(false);
            }
        }
    }
}

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

            if (argument == "вкл")
            {
                CyclicalMethodsManager.RunHackerChannelAutoChange();
                DataManager.HackerChannelIsRunning.Value = true;
                await DataManager.HackerChannelIsRunning.SaveAsync();
            }
            else if (argument == "выкл")
            {
                CyclicalMethodsManager.StopHackerChannelAutoChange();
                DataManager.HackerChannelIsRunning.Value = false;
                await DataManager.HackerChannelIsRunning.SaveAsync();
            }
        }
    }
}

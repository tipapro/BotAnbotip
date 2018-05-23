using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotAnbotip.Bot.Data;
using Discord;
using Discord.WebSocket;
using BotAnbotip.Bot.Data.Group;
using BotAnbotip.Bot.Clients;
using BotAnbotip.Bot.OtherModules;
using BotAnbotip.Bot.CyclicActions;

namespace BotAnbotip.Bot.Commands
{
    class RainbowRoleCommands
    {
        public static async Task ChangeRainbowRoleState(SocketMessage message, string argument)
        {
            await message.DeleteAsync();
            if (!CommandManager.CheckPermission((IGuildUser)message.Author, RoleIds.Основатель)) return;

            var str = argument.Split(' ');
            if (str.Length > 1)
            {
                argument = str[0];
                await DataManager.RainbowRoleId.SaveAsync(ulong.Parse(str[1]));
            }
            if (argument == "вкл")
            {
                CyclicActionManager.RainbowRoleAutoChange.Run();
                await DataManager.RainbowRoleIsRunning.SaveAsync(true);
            }
            else if (argument == "выкл")
            {
                CyclicActionManager.RainbowRoleAutoChange.Stop();
                await DataManager.RainbowRoleIsRunning.SaveAsync(false);
            }
        }       
    }
}

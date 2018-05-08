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
        public static async Task ChangeRainbowRoleState(string argument, SocketMessage message = null)
        {
            if (message != null)
            {
                await message.DeleteAsync();
                if (!CommandManager.CheckPermission((IGuildUser)message.Author, RoleIds.Основатель)) return;
            }

            var str = argument.Split(' ');
            if (str.Length > 1)
            {
                DataManager.RainbowRoleId.Value = ulong.Parse(str[1]);
                argument = str[0];
                await DataManager.RainbowRoleId.SaveAsync();
            }

            if (argument == "вкл")
            {
                CyclicalMethodsManager.RunRainbowRoleAutoChange();
                DataManager.RainbowRoleIsRunning.Value = true;
                await DataManager.RainbowRoleIsRunning.SaveAsync();
            }
            else if (argument == "выкл")
            {
                CyclicalMethodsManager.StopRainbowRoleAutoChange();
                DataManager.RainbowRoleIsRunning.Value = false;
                await DataManager.RainbowRoleIsRunning.SaveAsync();
            }
        }

        
    }
}

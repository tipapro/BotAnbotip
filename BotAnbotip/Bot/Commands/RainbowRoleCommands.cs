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
    class RainbowRoleCommands : CommandsBase
    {
        public RainbowRoleCommands() : base
            (
            (TransformMessageToChangeState,
            new string[] { "радуга", "rainbow" })
            ){ }

        private static async Task TransformMessageToChangeState(IMessage message, string argument)
        {
            await message.DeleteAsync();
            if (!CommandManager.CheckPermission((IGuildUser)message.Author, RoleIds.Основатель)) return;

            var strArray = argument.Split(' ');

            ulong roleId = 0;
            if (strArray.Length > 1)
            {
                argument = strArray[0];
                roleId = ulong.Parse(strArray[1]);
            }

            bool changedState = false;
            switch (argument)
            {
                case "вкл":
                case "+":
                case "on": changedState = true; break;
                case "выкл":
                case "-":
                case "off": changedState = false; break;
                default: throw new ArgumentException("Неопознанный аргумент", "changedState");
            }
            await CommandManager.RainbowRole.ChangeStateAsync(changedState, roleId);
        }

        public async Task ChangeStateAsync(bool changedState, ulong roleId = 0)
        {
            if (roleId != 0) await DataManager.RainbowRoleId.SaveAsync(roleId);

            if (changedState)
            {
                CyclicActionManager.RainbowRoleAutoChange.Run();
                await DataManager.RainbowRoleIsRunning.SaveAsync(true);
            }
            else
            {
                CyclicActionManager.RainbowRoleAutoChange.Stop();
                await DataManager.RainbowRoleIsRunning.SaveAsync(false);
            }
        }
    }
}

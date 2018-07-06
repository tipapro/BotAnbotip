using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BotAnbotip.Bot.Data;
using Discord.WebSocket;
using BotAnbotip.Bot.Data.Group;
using BotAnbotip.Bot.Clients;
using Discord;

namespace BotAnbotip.Bot.Commands
{
    class RoleManagementCommands : CommandsBase
    {
        public RoleManagementCommands() : base
            (
            (TransformMessageToGetAsync,
            new string[] { "дайроль", "получитьроль", "givemerole", "getrole" })
            ){ }

        private static async Task TransformMessageToGetAsync(IMessage message, string argument)
        {
            await message.DeleteAsync();
            if (!CommandManager.CheckPermission((IGuildUser)message.Author, RoleIds.Активный_Участник)) return;
            ulong roleId = ulong.Parse(argument.Substring(3, argument.Length - 4));
            await CommandManager.RoleManagement.GetAsync(message.Author, roleId);
        }

        public async Task GetAsync(IUser user, ulong roleId)
        {
            if (CheckTheRole(roleId))
            {
                await ((IGuildUser)user).AddRoleAsync(BotClientManager.MainBot.Guild.GetRole(roleId));
            }
        }

        private static bool CheckTheRole(ulong roleId) => 
            (roleId == (ulong)RoleIds.Любитель_Аниме) || 
            (roleId == (ulong)RoleIds._);
        
    }
}

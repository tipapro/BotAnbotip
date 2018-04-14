using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BotAnbotip.Bot.Data;
using Discord.WebSocket;

namespace BotAnbotip.Bot.Commands
{
    class ManageTheRolesCommands
    {
        public static async Task GetAsync(SocketMessage message, string argument)
        {
            await message.DeleteAsync();

            ulong roleId = 0;
            try
            {
                roleId = ulong.Parse(argument.Substring(3, argument.Length - 4));
            }
            catch
            {
                Console.WriteLine("Роль.....");
                return;
            }

            if (CheckTheRole(roleId))
            {
                await ((SocketGuildUser)message.Author).AddRoleAsync(Info.GroupGuild.GetRole(roleId));
            }
        }

        private static bool CheckTheRole(ulong roleId) => 
            (roleId == (ulong)RoleIds.Любитель_Аниме) || 
            (roleId == (ulong)RoleIds._);
        
    }
}

using BotAnbotip.Clients;
using BotAnbotip.Data.CustomEnums;
using BotAnbotip.Data.Group;
using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BotAnbotip.OtherModules
{
    class PunishmentModule
    {
        public static async Task Punish(ulong userId, PunishmentReason reason)
        {
            var user = (IUser)ClientControlManager.MainBot.Guild.GetUser(userId);
            await user.SendMessageAsync("Предупреждение: " + reason);
        }
    }
}

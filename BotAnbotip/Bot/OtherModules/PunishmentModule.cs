using BotAnbotip.Bot.Data.CustomEnums;
using BotAnbotip.Bot.Data.Group;
using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BotAnbotip.Bot.OtherModules
{
    class PunishmentModule
    {
        public static async Task Punish(ulong userId, PunishmentReason reason)
        {
            var user = (IUser)ConstInfo.MainGroupGuild.GetUser(userId);
            await user.SendMessageAsync("Предупреждение: " + reason);
        }
    }
}

using BotAnbotip.Data;
using BotAnbotip.Data.Group;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BotAnbotip.Commands
{
    class DebugCommands : CommandBase
    {
        public DebugCommands() : base
            (
            (TransformMessageToChangeFlagAsync,
            new string[] { "сменифлаг", "changeflag" })
            ){ }

        private static async Task TransformMessageToChangeFlagAsync(IMessage message, string argument)
        {
            await message.DeleteAsync();
            if (!CommandControlManager.CheckPermission((IGuildUser)message.Author, RoleIds.Founder)) return;
            var num = int.Parse(argument);
            await CommandControlManager.Debug.ChangeFlagAsync(num);
        }

        public Task ChangeFlagAsync(int num)
        {
            DataControlManager.DebugTriger[num] = !DataControlManager.DebugTriger[num];
            return Task.CompletedTask;
        }
    }
}

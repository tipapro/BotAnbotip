using BotAnbotip.Bot.Data;
using BotAnbotip.Bot.Data.Group;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BotAnbotip.Bot.Commands
{
    class DebugCommands : CommandsBase
    {
        public DebugCommands() : base
            (
            (TransformMessageToChangeFlagAsync,
            new string[] { "сменифлаг", "changeflag" })
            ){ }

        private static async Task TransformMessageToChangeFlagAsync(IMessage message, string argument)
        {
            await message.DeleteAsync();
            if (!CommandManager.CheckPermission((IGuildUser)message.Author, RoleIds.Founder)) return;
            var num = int.Parse(argument);
            await CommandManager.Debug.ChangeFlagAsync(num);
        }

        public Task ChangeFlagAsync(int num)
        {
            DataManager.DebugTriger[num] = !DataManager.DebugTriger[num];
            return Task.CompletedTask;
        }
    }
}

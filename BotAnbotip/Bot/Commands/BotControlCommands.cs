using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotAnbotip.Bot.Data;
using BotAnbotip.Bot.Data.Group;

namespace BotAnbotip.Bot.Commands
{
    class BotControlCommands
    {
        public static async void Stop(IMessage message, DiscordSocketClient client)
        {
            await message.DeleteAsync();
            if (!CommandManager.CheckPermission((IGuildUser)message.Author, RoleIds.Основатель)) return;

            await DataManager.SaveAllDataAsync();
            await client.StopAsync();
            Environment.Exit(0);
        }

        public static async void ClearData(IMessage message, DiscordSocketClient client)
        {
            await message.DeleteAsync();
            if (!CommandManager.CheckPermission((IGuildUser)message.Author, RoleIds.Основатель)) return;

            DataManager.InitializeAllVariables();
            await DataManager.SaveAllDataAsync();

        }
    }
}

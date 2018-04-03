using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotAnbotip.Bot.Data;

namespace BotAnbotip.Bot.Commands
{
    class BotControlCommands
    {
        public static async void Stop(SocketMessage message, DiscordSocketClient client)
        {
            await message.DeleteAsync();

            if (((IGuildUser)message.Author).RoleIds.Contains((ulong)RoleIds.Основатель))
            {
                DataManager.SaveData();
                await client.StopAsync();
                Environment.Exit(0);
            }
        }
    }
}

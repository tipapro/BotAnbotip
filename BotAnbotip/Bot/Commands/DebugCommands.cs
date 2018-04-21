using BotAnbotip.Bot.Data;
using BotAnbotip.Bot.Data.Group;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace BotAnbotip.Bot.Commands
{
    class DebugCommands
    {
        public static void ChangeFlag(SocketMessage message, int num)
        {
            message.DeleteAsync();
            if (!CommandManager.CheckPermission((IGuildUser)message.Author, RoleIds.Основатель)) return;
            DataManager.DebugTriger[num] = !DataManager.DebugTriger[num];
        }
    }
}

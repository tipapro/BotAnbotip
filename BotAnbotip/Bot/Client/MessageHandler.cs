using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BotAnbotip.Bot.Commands;
using Discord;
using BotAnbotip.Bot.Data;

namespace BotAnbotip.Bot.Client
{
    class MessageHandler
    {
        //private static Dictionary<ulong, DateTimeOffset> User
        public async Task MessageReceived(SocketMessage message)
        {
            if (message.Author.Id == BotClient.Client.CurrentUser.Id) return;

            if (message.Content.ToCharArray()[0] == PrivateData.Prefix)
            {
                string[] buf = message.Content.Substring(1).Split(' ');
                string command = buf[0];
                string argument = "";
                if (buf.Length > 1)
                {
                    argument = message.Content.Substring((PrivateData.Prefix + command + " ").ToCharArray().Length);
                }
                await CommandManager.RunCommand(command, argument, message);
            }
        }

        public void AntiSpam(IUser user)
        {

        }
    }
}

using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BotAnbotip.Bot.Commands;
using Discord;
using BotAnbotip.Bot.Data;
using BotAnbotip.Bot.Data.CustomEnums;
using BotAnbotip.Bot.Clients;
using BotAnbotip.Bot.OtherModules;

namespace BotAnbotip.Bot.Handlers
{
    class MessageHandler
    {
        private ulong botId;
        private char prefix;

        private CommandManager _cmdManager;

        public MessageHandler(ulong botId, char prefix)
        {
            this.botId = botId;
            this.prefix = prefix;
            _cmdManager = new CommandManager(botId);
        }

        public Task MessageReceived(SocketMessage message)
        {
            try
            {
                ProcessTheMessage(message);
            }
            catch (Exception ex)
            {
                new ExceptionLogger().Log(ex, "Ошибка при обработке отправленного сообщения");
            }
            return Task.CompletedTask;
        }

        private async void ProcessTheMessage(SocketMessage message)
        {
            if ((message.Content == "") || (message.Content == null) || (message.Author.Id == BotClientManager.AuxiliaryBot.Id)
                || (message.Author.Id == BotClientManager.MainBot.Id) || (message.Content.ToCharArray()[0] != prefix)) return;
            string[] buf = message.Content.Substring(1).Split(' ');
            string command = buf[0];
            string argument = "";
            if (buf.Length > 1) argument = message.Content.Substring((prefix + command + " ").ToCharArray().Length);
            await _cmdManager.RunCommand(command.ToLower(), argument, message);
        }
    }
}

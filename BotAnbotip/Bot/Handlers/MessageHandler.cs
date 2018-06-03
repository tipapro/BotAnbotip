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
        private AntiSpam antiSpam;
        private ulong botId;
        private char prefix;

        private CommandManager _cmdManager;

        public MessageHandler(ulong botId, char prefix)
        {
            antiSpam = new AntiSpam(SpamType.Message);
            this.botId = botId;
            this.prefix = prefix;
            _cmdManager = new CommandManager(botId);
        }

        public async Task MessageReceived(SocketMessage message)
        {
            try
            {
                await ProcessTheMessage(message);
            }
            catch (Exception ex)
            {
                new ExceptionLogger().Log(ex, "Ошибка при обработке отправленного сообщения");
            }
            //return Task.CompletedTask;
        }

        private async Task ProcessTheMessage(SocketMessage message)
        {
            if ((message.Content == "") || (message.Content == null) || (message.Author.Id == BotClientManager.AuxiliaryBot.Id)
                || (message.Author.Id == BotClientManager.MainBot.Id) || (antiSpam.Check(message.Author.Id, message.Content))
                || (message.Content.ToCharArray()[0] != prefix)) return;
            string[] buf = message.Content.Substring(1).Split(' ');
            string command = buf[0];
            string argument = "";
            if (buf.Length > 1) argument = message.Content.Substring((prefix + command + " ").ToCharArray().Length);
            await _cmdManager.RunCommand(command.ToLower(), argument, message);
        }
    }
}
